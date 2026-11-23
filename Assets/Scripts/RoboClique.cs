using UnityEngine;
using UnityEngine.EventSystems;

// Coloque no objeto "Robo" (precisa ter uma Image com Raycast Target ligado).
// Ao clicar:
//  - se o terminal ainda NAO foi concluido, abre o dialogo "Antes do Terminal"
//  - se o terminal JA foi concluido, abre o dialogo "Depois do Terminal"
public class RoboClique : MonoBehaviour, IPointerClickHandler
{
    [Header("Terminal (objeto ConteudoTerminal, que tem o TerminalSenhas)")]
    public TerminalSenhas terminal;

    [Header("Dialogos (DialogoTrigger com Gatilho = So Manual)")]
    public DialogoTrigger dialogoAntesDoTerminal;
    public DialogoTrigger dialogoDepoisDoTerminal;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Nao abre outro dialogo enquanto um ja esta tocando
        if (DialogoManager.EmDialogo) return;

        bool concluido = terminal != null && terminal.Concluido;
        DialogoTrigger alvo = concluido ? dialogoDepoisDoTerminal : dialogoAntesDoTerminal;

        if (alvo != null) alvo.Disparar();
    }
}