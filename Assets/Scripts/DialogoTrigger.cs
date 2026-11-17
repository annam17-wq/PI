using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogoTrigger : MonoBehaviour
{
    public enum Gatilho
    {
        AoIniciarCena,   // assim que a cena começa (ex.: apertou Jogar e a Sala1 abriu)
        AoAtivarObjeto,  // quando este objeto é ativado (ex.: um painel/sala que aparece ao trocar pela setinha)
        SoManual         // só quando alguém chamar Disparar() (botão, ItemPickup, UnityEvent...)
    }

    public DialogoData dialogo;
    public Gatilho gatilho = Gatilho.SoManual;
    public float atraso = 0f;
    public UnityEvent aoTerminar;

    void Start()
    {
        if (gatilho == Gatilho.AoIniciarCena) Disparar();
    }

    void OnEnable()
    {
        if (gatilho == Gatilho.AoAtivarObjeto) Disparar();
    }

    // Pode ser ligado em On Click() de botões ou em qualquer UnityEvent
    public void Disparar()
    {
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