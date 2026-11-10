using UnityEngine;
using UnityEngine.Events;

public class CasaDeBonecas : MonoBehaviour
{
    [Header("Boneca")]
    public string idBoneca = "Bonecaid";

    [Header("Ordem: cama, TV, fogao, banheiro, estante, cama")]
    public LugarBoneca[] sequencia;

    [Header("Quando completar a ordem")]
    public UnityEvent aoConcluir;

    private int etapa = 0;

    public bool Concluido
    {
        get { return sequencia != null && etapa >= sequencia.Length; }
    }

    public bool PodeColocarEm(LugarBoneca lugar)
    {
        if (sequencia == null || Concluido) return false;
        return sequencia[etapa] == lugar;
    }

    public void RegistrarColocacao()
    {
        etapa++;

        if (Concluido)
        {
            Debug.Log("Casa de bonecas: ordem completa!");
            if (aoConcluir != null) aoConcluir.Invoke();
        }
    }
}