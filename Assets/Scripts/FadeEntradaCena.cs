using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Coloque na cena que recebe a transição. Começa com a tela preta e clareia;
// ao terminar, dispara o evento (ex.: começar um diálogo).
public class FadeEntradaCena : MonoBehaviour
{
    public CanvasGroup telaPreta;
    public float pausaInicial = 0.3f;
    public float duracao = 1.5f;
    public UnityEvent aoTerminar;

    IEnumerator Start()
    {
        if (telaPreta == null)
        {
            Debug.LogError("[FadeEntradaCena] O campo 'Tela Preta' está vazio.");
            yield break;
        }

        PrepararTelaPreta();

        telaPreta.alpha = 1f;
        telaPreta.blocksRaycasts = true;
        Debug.Log("[FadeEntradaCena] Tela preta ligada, começando o fade.");

        yield return new WaitForSecondsRealtime(pausaInicial);

        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;
            telaPreta.alpha = Mathf.Lerp(1f, 0f, t / duracao);
            yield return null;
        }
        telaPreta.alpha = 0f;
        telaPreta.blocksRaycasts = false;

        aoTerminar?.Invoke();
    }

    // Garante que a tela preta cobre tudo e fica por cima de qualquer outra UI
    void PrepararTelaPreta()
    {
        telaPreta.gameObject.SetActive(true);

        RectTransform rt = telaPreta.transform as RectTransform;
        if (rt != null)
        {
            rt.SetAsLastSibling();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
            Vector3 p = rt.localPosition;
            p.z = 0f;
            rt.localPosition = p;
        }

        Canvas c = telaPreta.GetComponent<Canvas>();
        if (c == null) c = telaPreta.gameObject.AddComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = 999;

        if (telaPreta.GetComponent<GraphicRaycaster>() == null)
            telaPreta.gameObject.AddComponent<GraphicRaycaster>();
    }
}