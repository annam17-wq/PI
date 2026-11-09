using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int index;
    public Image icone;

    private Transform paiOriginal;
    private CanvasGroup canvasGroup;
    private Canvas canvasRaiz;
    private ItemPickup pickup;

    void Awake()
    {
        canvasGroup = icone.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = icone.gameObject.AddComponent<CanvasGroup>();

        canvasRaiz = GetComponentInParent<Canvas>();

        // Todo slot precisa de um ItemPickup no ícone para o item poder ser usado.
        // Se faltar (ou estiver diferente dos outros slots), criamos/configuramos aqui.
        pickup = icone.GetComponent<ItemPickup>();
        if (pickup == null) pickup = icone.gameObject.AddComponent<ItemPickup>();
        ConfigurarPickup();
    }

    // Deixa o ItemPickup do ícone igual em TODOS os slots, sem depender do Inspector
    private void ConfigurarPickup()
    {
        if (pickup == null) return;

        pickup.enabled = true;
        pickup.modo = ItemPickup.ModoInteracao.ApenasArrastar;
        pickup.arrastavel = true;

        // O id do item vem direto do slot (veja Preencher/Limpar).
        // Sem Database, o ItemPickup não tenta adivinhar o id pelo sprite do ícone.
        pickup.database = null;
    }

    public void Preencher(ItemData item)
    {
        icone.enabled = true;
        icone.sprite = item.icone;

        ConfigurarPickup();
        if (pickup != null) pickup.itemId = item.itemId;
    }

    public void Limpar()
    {
        icone.enabled = false;
        icone.sprite = null;

        if (pickup != null) pickup.itemId = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!icone.enabled) return;

        // Slots criados em tempo de execução (gaveta extra) podem precisar buscar o Canvas aqui
        if (canvasRaiz == null) canvasRaiz = GetComponentInParent<Canvas>();

        paiOriginal = icone.transform.parent;
        icone.transform.SetParent(canvasRaiz.transform);
        icone.transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!icone.enabled) return;
        icone.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (icone.transform.parent == canvasRaiz.transform)
        {
            icone.transform.SetParent(paiOriginal);
            icone.transform.localPosition = Vector3.zero;
        }
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI slotOrigem = eventData.pointerDrag.GetComponent<InventorySlotUI>();
        if (slotOrigem == null || slotOrigem == this) return;

        InventoryUI.Instance.TrocarItens(slotOrigem.index, this.index);

        slotOrigem.icone.transform.SetParent(slotOrigem.transform);
        slotOrigem.icone.transform.localPosition = Vector3.zero;
    }
}