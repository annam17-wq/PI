using UnityEngine;
using UnityEngine.EventSystems;

// Coloque no objeto "Robo" (precisa ter uma Image com Raycast Target ligado).
// Ao clicar no Robo, escolhe o dialogo conforme a situacao:
//  - terminal NAO concluido                  -> "Antes do Terminal"
//  - terminal concluido e jogador TEM o cha  -> "Com Cha"
//  - terminal concluido e jogador NAO tem    -> "Sem Cha"
// O dialogo de depois de entregar a planta (quando o Robo da o cha) continua
// sendo o "Dialogo Apos Interacao" do Item Pickup do Robo.
public class RoboClique : MonoBehaviour, IPointerClickHandler
{
    [Header("Terminal (objeto ConteudoTerminal, que tem o TerminalSenhas)")]
    public TerminalSenhas terminal;

    [Header("Item do cha (itemId do ItemData)")]
    public string itemIdDoCha = "ChaRoxoid";

    [Header("Dialogos (DialogoTrigger com Gatilho = So Manual)")]
    [Tooltip("Terminal ainda nao concluido")]
    public DialogoTrigger dialogoAntesDoTerminal;

    [Tooltip("Terminal concluido e o jogador JA TEM o cha")]
    public DialogoTrigger dialogoComCha;

    [Tooltip("Terminal concluido e o jogador AINDA NAO tem o cha (ex.: vai fazer o cha)")]
    public DialogoTrigger dialogoSemCha;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Nao abre outro dialogo enquanto um ja esta tocando
        if (DialogoManager.EmDialogo) return;

        bool concluido = terminal != null && terminal.Concluido;

        DialogoTrigger alvo;
        if (!concluido) alvo = dialogoAntesDoTerminal;
        else alvo = TemCha() ? dialogoComCha : dialogoSemCha;

        if (alvo != null) alvo.Disparar();
    }

    bool TemCha()
    {
        if (GameManager.Instance == null || GameManager.Instance.currentData == null) return false;
        if (GameManager.Instance.currentData.inventory == null) return false;
        if (string.IsNullOrEmpty(itemIdDoCha)) return false;

        return GameManager.Instance.currentData.inventory
            .Exists(i => i != null && i.itemId == itemIdDoCha);
    }
}