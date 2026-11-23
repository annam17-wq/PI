using UnityEngine;
using UnityEngine.EventSystems;

// Coloque no MESMO objeto do ItemPickup que recebe a chave (ex.: "cadeado geladeira").
// Quando a chave for usada, o frigobar abre. Depois disso, clicar nele alterna entre aberto e fechado.
[RequireComponent(typeof(ItemPickup))]
public class FrigobarAlternar : MonoBehaviour, IPointerClickHandler
{
    [Header("Aparecem so com o frigobar FECHADO")]
    [Tooltip("Ex.: o PNG do frigobar fechado e a Lista")]
    public GameObject[] mostrarFechado;

    [Header("Aparecem so com o frigobar ABERTO")]
    [Tooltip("Ex.: o PNG do frigobar aberto e o item de dentro")]
    public GameObject[] mostrarAberto;

    [Header("Opcional")]
    [Tooltip("Dispara na primeira vez que o frigobar abre (use um gatilho ativo na raiz)")]
    public DialogoTrigger dialogoAoAbrirPrimeiraVez;

    private ItemPickup chave;
    private bool destrancado;
    private bool aberto;

    void Awake()
    {
        chave = GetComponent<ItemPickup>();
    }

    void Start()
    {
        // Estado inicial: fechado
        Aplicar(false);
    }

    void Update()
    {
        // Espera a chave ser usada (o ItemPickup marca a interacao como concluida)
        if (destrancado || chave == null || !chave.InteracaoConcluida) return;

        destrancado = true;
        Aplicar(true);

        if (dialogoAoAbrirPrimeiraVez != null)
            dialogoAoAbrirPrimeiraVez.Disparar();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Trancado: o clique fica por conta do ItemPickup (ex.: dialogo de trancado)
        if (!destrancado) return;

        Aplicar(!aberto);
    }

    private void Aplicar(bool abrir)
    {
        aberto = abrir;
        Definir(mostrarFechado, !abrir);
        Definir(mostrarAberto, abrir);
    }

    private static void Definir(GameObject[] lista, bool ativo)
    {
        if (lista == null) return;

        foreach (GameObject obj in lista)
            if (obj != null) obj.SetActive(ativo);
    }
}