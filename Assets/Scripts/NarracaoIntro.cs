using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Sequência: menu some -> tela escurece -> texto narrado no meio -> tela acende -> dispara o diálogo.
public class NarracaoIntro : MonoBehaviour
{
    [Header("Referências")]
    public FadeMenu menu;            // objeto MenuInicial (tem o FadeMenu)
    public CanvasGroup telaPreta;    // Image preta em tela cheia, com CanvasGroup
    public TMP_Text textoNarracao;   // texto no meio da tela (filho da TelaPreta)

    [Header("Tempos (segundos)")]
    public float duracaoEscurecer = 1.5f;
    public float duracaoTexto = 0.8f;
    public float pausaAntesDeAcender = 0.5f;
    public float duracaoClarear = 1.5f;

    [Header("Textos (cada item é uma tela; avança com espaço ou clique)")]
    [TextArea(3, 8)]
    public string[] paginas =
    {
        "30/10/2028\n\nEm uma véspera de Halloween, em uma fazendinha de abóboras no interior do Canadá, em Surrey, no bairro Cloverdale, Milla Salvatore e Agatha Martins estão escolhendo a abóbora perfeita para o Halloween."
    };

    public UnityEvent aoTerminar;

    bool rodando;

    // Ligue no On Click() do botão Jogar
    public void Iniciar()
    {
        if (!rodando) StartCoroutine(Sequencia());
    }

    IEnumerator Sequencia()
    {
        rodando = true;

        // 1. menu (título e botões) some
        if (menu != null)
        {
            menu.FadeOut();
            yield return new WaitForSeconds(menu.duracao);
        }

        // 2. tela escurece
        textoNarracao.alpha = 0f;
        telaPreta.blocksRaycasts = true;
        yield return FadeGrupo(telaPreta, 0f, 1f, duracaoEscurecer);

        // 3. textos
        foreach (string p in paginas)
        {
            textoNarracao.text = p;
            yield return FadeTexto(0f, 1f, duracaoTexto);
            yield return EsperarEntrada();
            yield return FadeTexto(1f, 0f, duracaoTexto);
        }

        yield return new WaitForSeconds(pausaAntesDeAcender);

        // 4. tela acende
        yield return FadeGrupo(telaPreta, 1f, 0f, duracaoClarear);
        telaPreta.blocksRaycasts = false;

        rodando = false;
        aoTerminar?.Invoke();
    }

    IEnumerator EsperarEntrada()
    {
        yield return null; // não aproveita um clique do mesmo frame
        while (!(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            yield return null;
    }

    IEnumerator FadeGrupo(CanvasGroup g, float de, float para, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            g.alpha = Mathf.Lerp(de, para, t / dur);
            yield return null;
        }
        g.alpha = para;
    }

    IEnumerator FadeTexto(float de, float para, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            textoNarracao.alpha = Mathf.Lerp(de, para, t / dur);
            yield return null;
        }
        textoNarracao.alpha = para;
    }
}