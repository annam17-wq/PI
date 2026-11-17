using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Escurece a tela (fade para preto) e só então carrega a cena.
public class TransicaoCena : MonoBehaviour
{
    public CanvasGroup telaPreta;
    public string nomeCena = "Sala1";
    public float duracao = 1.5f;

    bool rodando;

    // Ligue em eventos (ex.: Ao Terminar do DialogoTrigger)
    public void IrParaCena()
    {
        if (!rodando) StartCoroutine(Rotina());
    }

    IEnumerator Rotina()
    {
        rodando = true;
        telaPreta.blocksRaycasts = true;

        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;
            telaPreta.alpha = Mathf.Lerp(0f, 1f, t / duracao);
            yield return null;
        }
        telaPreta.alpha = 1f;

        SceneManager.LoadScene(nomeCena);
    }
}