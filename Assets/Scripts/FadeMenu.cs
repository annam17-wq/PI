using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Coloque no objeto MenuInicial (o CanvasGroup e adicionado sozinho).
// Faz fade em tudo que esta dentro: titulo e botoes.
[RequireComponent(typeof(CanvasGroup))]
public class FadeMenu : MonoBehaviour
{
    public float duracao = 1.5f;
    public bool fadeInAoAbrir = true;
    public UnityEvent aoTerminarFade;

    CanvasGroup grupo;
    bool rodando;

    void Awake()
    {
        grupo = GetComponent<CanvasGroup>();

        if (fadeInAoAbrir)
        {
            grupo.alpha = 0f;            // comeca invisivel
            grupo.interactable = false;
            grupo.blocksRaycasts = false;
        }
    }

    void Start()
    {
        if (fadeInAoAbrir) FadeIn();
    }

    // Some (alpha 1 -> 0)
    public void FadeOut()
    {
        if (!rodando) StartCoroutine(Fade(1f, 0f));
    }

    // Aparece (alpha 0 -> 1)
    public void FadeIn()
    {
        if (!rodando) StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float de, float para)
    {
        rodando = true;
        grupo.interactable = false;
        grupo.blocksRaycasts = false; // evita cliques durante o fade

        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;
            grupo.alpha = Mathf.Lerp(de, para, t / duracao);
            yield return null;
        }
        grupo.alpha = para;

        if (para > 0f)
        {
            grupo.interactable = true;
            grupo.blocksRaycasts = true;
        }

        rodando = false;
        aoTerminarFade?.Invoke();
    }
}