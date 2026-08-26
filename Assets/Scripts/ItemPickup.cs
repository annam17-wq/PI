using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPickup : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Precisa ser exatamente igual ao itemId cadastrado no ItemData")]
    public string itemId;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Cliquei no urso!"); // linha temporária de teste

        bool coletou = GameManager.Instance.AddItem(itemId);

        if (coletou)
        {
            gameObject.SetActive(false);
        }
    }
}