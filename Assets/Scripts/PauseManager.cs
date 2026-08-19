using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Painel mostrado durante o pause")]
    public GameObject painelPause;

    [Header("Tecla alternativa para pausar (opcional)")]
    public KeyCode teclaPause = KeyCode.Escape;

    [Header("Bloqueio de interação fora do pause")]
    [Tooltip("CanvasGroup que engloba todo o resto da interface do jogo (HUD, botões, etc). Deixe vazio se não quiser bloquear nada além do próprio EventSystem.")]
    public CanvasGroup canvasGrupoJogo;

    [Tooltip("Bloqueador de clique em tela cheia (opcional). Se atribuído, será ativado junto com o pause para capturar cliques em qualquer lugar da tela, mesmo fora de elementos de UI.")]
    public GameObject bloqueadorCliqueTelaCheia;

    private bool pausado = false;

    void Update()
    {
        // Permite pausar/despausar também pela tecla, além do botão
        if (Input.GetKeyDown(teclaPause))
        {
            AlternarPause();
        }
    }

    // Chame esse método no OnClick() do botão de pause
    public void AlternarPause()
    {
        pausado = !pausado;
        AplicarEstadoPause();
    }

    // Útil pro botão "Continuar" dentro do próprio painel de pause
    public void Continuar()
    {
        pausado = false;
        AplicarEstadoPause();
    }

    // Útil pro botão "Pausar" chamar direto, sem alternar
    public void Pausar()
    {
        pausado = true;
        AplicarEstadoPause();
    }

    // Centraliza a lógica de pause/despause e o bloqueio de interação
    private void AplicarEstadoPause()
    {
        Time.timeScale = pausado ? 0f : 1f;

        if (painelPause != null)
            painelPause.SetActive(pausado);

        BloquearInteracaoForaDoPainel(pausado);
    }

    // Bloqueia (ou libera) qualquer interação fora do painel de pause
    private void BloquearInteracaoForaDoPainel(bool bloquear)
    {
        // Bloqueia o resto da UI (HUD, botões do jogo, etc.)
        if (canvasGrupoJogo != null)
        {
            canvasGrupoJogo.interactable = !bloquear;
            canvasGrupoJogo.blocksRaycasts = !bloquear;
        }

        // Ativa um bloqueador de tela cheia atrás do painel de pause,
        // útil para capturar cliques/toques que não sejam em elementos de UI
        if (bloqueadorCliqueTelaCheia != null)
            bloqueadorCliqueTelaCheia.SetActive(bloquear);
    }
}