using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Coloque no objeto "NarracaoFinal" (filho do Canvas, por ULTIMO na lista, para ficar na frente de tudo).
// O objeto precisa ter: CanvasGroup + Image preta esticada na tela + um texto TMP filho (Texto).
//
// Como funciona:
//  1) Liberar()  -> chamado quando o ultimo dialogo do Robo termina (Ao Terminar do DialogoTrigger)
//  2) Iniciar()  -> chamado quando o jogador clica no "chao" no inventario
//     So comeca se ja foi liberado e se nenhum dialogo estiver aberto.
//  A tela escurece (fade in) e as frases aparecem UMA POR VEZ, letra por letra,
//  em texto branco no meio da tela.
//  Clique/Espaco: completa a frase que esta aparecendo; de novo: vai para a proxima.
//  Depois da ultima frase, o proximo clique dispara "Ao Terminar".
public class NarracaoFinal : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("CanvasGroup da tela preta (o proprio objeto)")]
    public CanvasGroup telaPreta;
    [Tooltip("Texto branco no meio da tela (TextMeshPro)")]
    public TMP_Text texto;

    [Header("Frases (uma por vez, na ordem)")]
    [TextArea(1, 4)]
    public string[] frases = { "Primeira frase.", "Segunda frase." };

    [Header("Tempos")]
    public float duracaoFade = 1.5f;
    public float pausaAntesDoTexto = 0.8f;
    public float segundosPorLetra = 0.05f;
    [Tooltip("Pausa curta com a tela vazia entre uma frase e outra")]
    public float pausaEntreFrases = 0.3f;

    [Header("Liberacao")]
    [Tooltip("Marcado: so comeca depois que Liberar() for chamado")]
    public bool exigirLiberacao = true;

    [Header("Quando terminar")]
    [Tooltip("Nome da cena para carregar no fim da narracao (ex.: TelaInicial). Vazio = nao troca de cena")]
    public string cenaAoTerminar = "TelaInicial";
    public UnityEvent aoTerminar;

    bool liberado;
    bool rodando;
    bool digitando;
    bool pularDigitacao;

    public bool Liberado { get { return liberado; } }

    void Awake()
    {
        if (telaPreta != null)
        {
            telaPreta.alpha = 0f;
            telaPreta.blocksRaycasts = false;
            telaPreta.interactable = false;
        }
        if (texto != null) texto.text = "";
    }

    // Ligar em "Ao Terminar" do dialogo final do Robo
    public void Liberar()
    {
        liberado = true;
    }

    // Ligar no clique do "chao" no inventario
    public void Iniciar()
    {
        if (rodando) return;
        if (exigirLiberacao && !liberado) return;
        if (DialogoManager.EmDialogo) return;
        if (telaPreta == null || texto == null) return;
        if (frases == null || frases.Length == 0) return;

        StartCoroutine(Rotina());
    }

    void Update()
    {
        if (!rodando) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (digitando) pularDigitacao = true;
        }
    }

    IEnumerator Rotina()
    {
        rodando = true;

        // Bloqueia cliques no resto do jogo enquanto a narracao roda
        telaPreta.blocksRaycasts = true;
        telaPreta.interactable = true;
        texto.text = "";

        // Fade in da tela preta
        float t = 0f;
        while (t < duracaoFade)
        {
            t += Time.unscaledDeltaTime;
            telaPreta.alpha = Mathf.Lerp(0f, 1f, t / duracaoFade);
            yield return null;
        }
        telaPreta.alpha = 1f;

        yield return new WaitForSecondsRealtime(pausaAntesDoTexto);

        for (int f = 0; f < frases.Length; f++)
        {
            // Frase aparecendo letra por letra
            texto.text = frases[f];
            texto.maxVisibleCharacters = 0;
            texto.ForceMeshUpdate();
            int total = texto.textInfo.characterCount;

            digitando = true;
            pularDigitacao = false;

            for (int i = 0; i <= total && !pularDigitacao; i++)
            {
                texto.maxVisibleCharacters = i;
                yield return new WaitForSecondsRealtime(segundosPorLetra);
            }

            texto.maxVisibleCharacters = int.MaxValue;
            digitando = false;

            // Espera um frame para o clique que completou a frase nao contar de novo
            yield return null;

            // Espera o jogador clicar/espaco para ir para a proxima
            while (!(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
                yield return null;

            // Tela vazia por um instante antes da proxima frase
            if (f < frases.Length - 1)
            {
                texto.text = "";
                if (pausaEntreFrases > 0f)
                    yield return new WaitForSecondsRealtime(pausaEntreFrases);
            }
        }

        rodando = false;
        if (aoTerminar != null) aoTerminar.Invoke();

        if (!string.IsNullOrEmpty(cenaAoTerminar))
            SceneManager.LoadScene(cenaAoTerminar);
    }
}