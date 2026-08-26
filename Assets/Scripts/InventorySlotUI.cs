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

    void Awake()
    {
        canvasGroup = icone.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = icone.gameObject.AddComponent<CanvasGroup>();

        canvasRaiz = GetComponentInParent<Canvas>();
    }

    public void Preencher(ItemData item)
    {
        icone.enabled = true;
        icone.sprite = item.icone;
    }

    public void Limpar()
    {
        icone.enabled = false;
        icone.sprite = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!icone.enabled) return;

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