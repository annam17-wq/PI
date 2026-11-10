using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LugarBoneca : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public CasaDeBonecas casa;

    [Tooltip("Sprite da boneca neste lugar")]
    public Sprite spriteBoneca;

    private Image imagem;
    private bool ocupado;

    void Awake()
    {
        Iniciar();
        MostrarVazio();
    }

    private void Iniciar()
    {
        if (imagem != null) return;
        imagem = GetComponent<Image>();
        imagem.preserveAspect = true;
        imagem.raycastTarget = true;
    }

    // Boneca solta neste lugar
    public void OnDrop(PointerEventData eventData)
    {
        Iniciar();

        if (ocupado || casa == null) return;
        if (eventData.pointerDrag == null) return;

        ItemPickup item = eventData.pointerDrag.GetComponent<ItemPickup>();
        if (item == null || item.itemId != casa.idBoneca) return;

        if (!casa.PodeColocarEm(this))
        {
            Debug.Log("[" + name + "] Esse nao e o proximo lugar da ordem.");
            return;
        }

        RemoverDoInventario(casa.idBoneca);
        item.MarcarComoConsumido();

        MostrarBoneca();
        casa.RegistrarColocacao();
    }

    // Clicar na boneca devolve ela ao inventario
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!ocupado || casa == null || casa.Concluido) return;

        if (!GameManager.Instance.AddItem(casa.idBoneca))
        {
            Debug.LogWarning("Inventario cheio, nao deu pra pegar a boneca.");
            return;
        }

        MostrarVazio();

        if (InventoryUI.Instance != null) InventoryUI.Instance.AtualizarUI();
    }

    private void MostrarBoneca()
    {
        Iniciar();
        ocupado = true;
        imagem.sprite = spriteBoneca;
        imagem.color = Color.white;
    }

    private void MostrarVazio()
    {
        Iniciar();
        ocupado = false;
        imagem.sprite = null;
        imagem.color = new Color(1f, 1f, 1f, 0f); // invisivel, mas ainda recebe clique/drop
    }

    private void RemoverDoInventario(string id)
    {
        foreach (var it in GameManager.Instance.currentData.inventory)
        {
            if (it != null && it.itemId == id)
            {
                it.itemId = "";
                break;
            }
        }
    }
}