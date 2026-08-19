using UnityEngine;
using UnityEngine.SceneManagement; // necessário para trocar de cena

public class PauseManager : MonoBehaviour
{
    [Header("Painel mostrado durante o pause")]
    public GameObject painelPause;

    [Header("Tecla alternativa para pausar (opcional)")]
    public KeyCode teclaPause = KeyCode.Escape;

    [Header("Bloqueio de interação fora do pause")]
    public CanvasGroup canvasGrupoJogo;
    public GameObject bloqueadorCliqueTelaCheia;

    private bool pausado = false;

    void Awake()
    {
        // Garantia extra: toda vez que essa cena carregar,
        // o jogo NUNCA deve começar pausado, mesmo que o
        // timeScale tenha ficado em 0 da cena anterior.
        Time.timeScale = 1f;
        pausado = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaPause))
        {
            AlternarPause();
        }
    }

    public void AlternarPause()
    {
        pausado = !pausado;
        AplicarEstadoPause();
    }

    public void Continuar()
    {
        pausado = false;
        AplicarEstadoPause();
    }

    public void Pausar()
    {
        pausado = true;
        AplicarEstadoPause();
    }

    // Chame esse método no OnClick() do botão "Voltar ao Menu"
    public void VoltarAoMenu(string nomeDaCena)
    {
        // Reseta TUDO antes de trocar de cena, senão o estado
        // de pause "vaza" pra próxima cena (menu principal)
        pausado = false;
        Time.timeScale = 1f;
        BloquearInteracaoForaDoPainel(false);

        SceneManager.LoadScene(nomeDaCena);
    }

    private void AplicarEstadoPause()
    {
        Time.timeScale = pausado ? 0f : 1f;

        if (painelPause != null)
            painelPause.SetActive(pausado);

        BloquearInteracaoForaDoPainel(pausado);
    }

    private void BloquearInteracaoForaDoPainel(bool bloquear)
    {
        if (canvasGrupoJogo != null)
        {
            canvasGrupoJogo.interactable = !bloquear;
            canvasGrupoJogo.blocksRaycasts = !bloquear;
        }

        if (bloqueadorCliqueTelaCheia != null)
            bloqueadorCliqueTelaCheia.SetActive(bloquear);
    }
}