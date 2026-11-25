using System.Collections;
using UnityEngine;

// Coloque numa Image preta que cobre a tela toda, por cima de tudo.
[RequireComponent(typeof(CanvasGroup))]
public class FadeInCena : MonoBehaviour
{
    public float duracao = 2f;
    public float esperaAntes = 0.3f; // segura o preto um instante

    IEnumerator Start()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float espera = 0f;
        while (espera < esperaAntes)
        {
            espera += Time.unscaledDeltaTime;
            yield return null;
        }

        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / duracao);
            yield return null;
        }

        cg.alpha = 0f;
        gameObject.SetActive(false);
    }
}