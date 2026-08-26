using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPickup : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Precisa ser exatamente igual ao itemId cadastrado no ItemData")]
    public string itemId;

    void Start()
    {
        // Se esse item já foi coletado antes (está salvo no inventário),
        // o objeto nem deve aparecer na cena.
        if (JaFoiColetado())
        {
            gameObject.SetActive(false);
        }
    }

    private bool JaFoiColetado()
    {
        var inventario = GameManager.Instance.currentData.inventory;
        return inventario.Exists(i => i.itemId == itemId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool coletou = GameManager.Instance.AddItem(itemId);
        if (coletou)
        {
            gameObject.SetActive(false);
        }
    }
}