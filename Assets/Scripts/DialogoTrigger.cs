using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogoTrigger : MonoBehaviour
{
    public enum Gatilho
    {
        AoIniciarCena,   // assim que a cena comeca (ex.: apertou Jogar e a Sala1 abriu)
        AoAtivarObjeto,  // quando este objeto e ativado (ex.: um painel/sala que aparece ao trocar pela setinha)
        SoManual         // so quando alguem chamar Disparar() (botao, ItemPickup, UnityEvent...)
    }

    public DialogoData dialogo;
    public Gatilho gatilho = Gatilho.SoManual;
    public float atraso = 0f;

    [Tooltip("Opcional: so dispara se este terminal ja tiver sido concluido (ex.: ConteudoTerminal). Vazio = sem condicao")]
    public TerminalSenhas exigeTerminalConcluido;

    [Tooltip("Opcional: NAO dispara se este terminal ja tiver sido concluido (ex.: dialogo que so vale antes do fim do jogo). Vazio = sem condicao")]
    public TerminalSenhas naoDisparaSeTerminalConcluido;

    public UnityEvent aoTerminar;

    void Start()
    {
        if (gatilho == Gatilho.AoIniciarCena) Disparar();
    }

    void OnEnable()
    {
        if (gatilho == Gatilho.AoAtivarObjeto) Disparar();
    }

    // Pode ser ligado em On Click() de botoes ou em qualquer UnityEvent
    public void Disparar()
    {
        if (exigeTerminalConcluido != null && !exigeTerminalConcluido.Concluido) return;
        if (naoDisparaSeTerminalConcluido != null && naoDisparaSeTerminalConcluido.Concluido) return;

        if (isActiveAndEnabled) StartCoroutine(Rotina());
    }

    IEnumerator Rotina()
    {
        if (atraso > 0f) yield return new WaitForSeconds(atraso);
        else yield return null; // espera 1 frame para o DialogoManager estar pronto

        if (DialogoManager.Instance != null)
            DialogoManager.Instance.IniciarDialogo(dialogo, () => aoTerminar?.Invoke());
    }
}