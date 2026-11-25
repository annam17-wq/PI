using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Coloque este script em um objeto que NÃO seja o painel (ex.: Managers),
// senão ele é desativado junto com o painel e para de funcionar.
public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance { get; private set; }
    public static bool EmDialogo { get; private set; }

    [Header("UI")]
    public GameObject painel;            // raiz: fundo transparente em tela cheia + balão + personagem
    public TMP_Text textoNome;
    public TMP_Text textoFala;
    public Image imagemPersonagem;
    public GameObject indicadorContinuar; // opcional (setinha piscando)

    [Header("Velocidade")]
    public float segundosPorLetra = 0.03f;
    public float intervaloBoca = 0.12f;

    // Reserva usada só se não houver GameManager na cena (ex.: testando uma cena isolada)
    static readonly HashSet<string> vistosReserva = new HashSet<string>();

    DialogoData atual;
    int indice;
    bool digitando;
    Coroutine rotina;
    System.Action aoTerminar;
    int frameInicio;

    void Awake()
    {
        Instance = this;
        EmDialogo = false;
        if (painel != null) painel.SetActive(false);
    }

    // Os diálogos já vistos ficam em GameManager.currentData.dialogosVistos,
    // então são gravados junto com o save (botão Salvar) e carregados de volta.
    static List<string> ListaVistos()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.currentData == null) return null;
        if (gm.currentData.dialogosVistos == null)
            gm.currentData.dialogosVistos = new List<string>(); // saves antigos não têm a lista
        return gm.currentData.dialogosVistos;
    }

    public static bool JaViu(string nome)
    {
        var l = ListaVistos();
        return l != null ? l.Contains(nome) : vistosReserva.Contains(nome);
    }

    static void MarcarVisto(string nome)
    {
        var l = ListaVistos();
        if (l != null) { if (!l.Contains(nome)) l.Add(nome); }
        else vistosReserva.Add(nome);
    }

    // Use só ao começar um JOGO NOVO de verdade (apaga o histórico de diálogos vistos).
    public static void ResetarVistos()
    {
        var l = ListaVistos();
        if (l != null) l.Clear();
        vistosReserva.Clear();
        EmDialogo = false;
    }

    public void IniciarDialogo(DialogoData dialogo, System.Action fim = null)
    {
        if (EmDialogo || dialogo == null || dialogo.falas.Count == 0) return;
        if (dialogo.soUmaVez && JaViu(dialogo.name)) return;
        if (dialogo.soUmaVez) MarcarVisto(dialogo.name);

        atual = dialogo;
        aoTerminar = fim;
        indice = 0;
        EmDialogo = true;
        frameInicio = Time.frameCount; // evita que o clique que disparou o diálogo já avance a fala
        painel.SetActive(true);
        MostrarFala();
    }

    void Update()
    {
        if (!EmDialogo || Time.frameCount <= frameInicio) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (digitando)
            {
                StopCoroutine(rotina);
                FinalizarDigitacao(atual.falas[indice]);
            }
            else
            {
                ProximaFala();
            }
        }
    }

    void MostrarFala()
    {
        Fala f = atual.falas[indice];
        textoNome.text = f.nome;
        TrocarBoca(f, false);
        if (indicadorContinuar != null) indicadorContinuar.SetActive(false);
        rotina = StartCoroutine(Digitar(f));
    }

    IEnumerator Digitar(Fala f)
    {
        digitando = true;
        textoFala.text = f.texto;
        textoFala.maxVisibleCharacters = 0;
        textoFala.ForceMeshUpdate();
        int total = textoFala.textInfo.characterCount;

        float tBoca = 0f;
        bool aberta = false;

        for (int i = 0; i <= total; i++)
        {
            textoFala.maxVisibleCharacters = i;
            tBoca += segundosPorLetra;
            if (tBoca >= intervaloBoca)
            {
                tBoca = 0f;
                aberta = !aberta;
                TrocarBoca(f, aberta);
            }
            yield return new WaitForSeconds(segundosPorLetra);
        }
        FinalizarDigitacao(f);
    }

    void FinalizarDigitacao(Fala f)
    {
        textoFala.maxVisibleCharacters = int.MaxValue;
        digitando = false;
        TrocarBoca(f, false);
        if (indicadorContinuar != null) indicadorContinuar.SetActive(true);
    }

    void TrocarBoca(Fala f, bool aberta)
    {
        Sprite s = (aberta && f.spriteAberto != null) ? f.spriteAberto : f.spriteFechado;
        imagemPersonagem.sprite = s;
        imagemPersonagem.enabled = s != null;
    }

    void ProximaFala()
    {
        indice++;
        if (indice >= atual.falas.Count) Encerrar();
        else MostrarFala();
    }

    void Encerrar()
    {
        EmDialogo = false;
        painel.SetActive(false);
        var fim = aoTerminar;
        aoTerminar = null;
        fim?.Invoke();
    }
}