using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPickup : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Precisa ser exatamente igual ao itemId cadastrado no ItemData")]
    public string itemId;

    void Start()
    {
        // Verifica se o GameManager existe.
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                $"[{gameObject.name}] GameManager.Instance é null! " +
                "Verifique se existe um GameManager ativo na cena.",
                this
            );

            enabled = false;
            return;
        }

        // Verifica se os dados do jogo foram inicializados.
        if (GameManager.Instance.currentData == null)
        {
            Debug.LogError(
                $"[{gameObject.name}] currentData está null!",
                this
            );

            enabled = false;
            return;
        }

        // Verifica se o inventário foi inicializado.
        if (GameManager.Instance.currentData.inventory == null)
        {
            Debug.LogError(
                $"[{gameObject.name}] inventory está null!",
                this
            );

            enabled = false;
            return;
        }

        // Se já foi coletado, não deve aparecer na cena.
        if (JaFoiColetado())
        {
            gameObject.SetActive(false);
        }
    }

    private bool JaFoiColetado()
    {
        var inventario = GameManager.Instance.currentData.inventory;

        return inventario.Exists(
            i => i != null && i.itemId == itemId
        );
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Impede a coleta se o GameManager não estiver disponível.
        if (GameManager.Instance == null)
        {
            Debug.LogError("Não foi possível coletar: GameManager está null!");
            return;
        }

        // Tenta adicionar o item ao inventário.
        bool coletou = GameManager.Instance.AddItem(itemId);

        if (coletou)
        {
            Debug.Log($"Item coletado: {itemId}");
            gameObject.SetActive(false);
        }
    }
}