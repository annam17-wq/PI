using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZoomManager : MonoBehaviour
{
    public static ZoomManager Instance;
    public static bool EstaAberto => Instance != null && Instance.aberto;

    [Header("Referências")]
    public CanvasGroup painelZoom;
    public Image imagemAmpliada;
    public GameObject conteudoTerminal;
    public Button setaVoltar;

    [Header("Config")]
    public float duracaoFade = 0.25f;

    bool aberto;
    Coroutine rotina;
    GameObject extraAtual; // conteúdo extra do zoom atual (ex.: parte clicável que dá item)

    void Awake()
    {
        Instance = this;
        if (setaVoltar != null) setaVoltar.onClick.AddListener(Fechar);
        painelZoom.alpha = 0f;
        painelZoom.gameObject.SetActive(false);
    }

    // conteudoExtra é opcional: objeto que só aparece enquanto este zoom está aberto
    public void AbrirImagem(Sprite sprite, GameObject conteudoExtra = null)
    {
        if (sprite == null) return;
        EsconderExtra();

        imagemAmpliada.sprite = sprite;
        imagemAmpliada.gameObject.SetActive(true);
        if (conteudoTerminal != null) conteudoTerminal.SetActive(false);

        extraAtual = conteudoExtra;
        if (extraAtual != null) extraAtual.SetActive(true);

        Mostrar();
    }

    public void AbrirTerminal()
    {
        EsconderExtra();
        imagemAmpliada.gameObject.SetActive(false);
        if (conteudoTerminal != null) conteudoTerminal.SetActive(true);
        Mostrar();
    }

    public void Fechar()
    {
        if (!aberto) return;
        aberto = false;
        Trocar(false);
    }

    void EsconderExtra()
    {
        if (extraAtual != null) extraAtual.SetActive(false);
        extraAtual = null;
    }

    void Mostrar()
    {
        aberto = true;
        painelZoom.gameObject.SetActive(true);
        Trocar(true);
    }

    void Trocar(bool abrir)
    {
        if (rotina != null) StopCoroutine(rotina);
        rotina = StartCoroutine(Fade(abrir ? 1f : 0f, abrir));
    }

    IEnumerator Fade(float alvo, bool abrir)
    {
        float inicio = painelZoom.alpha, t = 0f;
        painelZoom.interactable = abrir;
        painelZoom.blocksRaycasts = abrir;
        while (t < duracaoFade)
        {
            t += Time.unscaledDeltaTime;
            painelZoom.alpha = Mathf.Lerp(inicio, alvo, t / duracaoFade);
            yield return null;
        }
        painelZoom.alpha = alvo;
        if (!abrir)
        {
            EsconderExtra();
            painelZoom.gameObject.SetActive(false);
        }
    }
}