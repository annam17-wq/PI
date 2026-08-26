using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public ItemDatabase database;
    public List<InventorySlotUI> slots; // arraste os 5 slots aqui, na ordem

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AtualizarUI();
    }

    public void AtualizarUI()
    {
        List<InventoryItem> inventario = GameManager.Instance.currentData.inventory;

        for (int i = 0; i < slots.Count; i++)
        {
            InventoryItem dados = inventario[i];

            if (dados != null && dados.itemId != "")
            {
                ItemData item = database.GetItem(dados.itemId);
                if (item != null)
                    slots[i].Preencher(item);
                else
                    slots[i].Limpar();
            }
            else
            {
                slots[i].Limpar();
            }
        }
    }

    public void TrocarItens(int indexA, int indexB)
    {
        var inventario = GameManager.Instance.currentData.inventory;

        InventoryItem temp = inventario[indexA];
        inventario[indexA] = inventario[indexB];
        inventario[indexB] = temp;

        AtualizarUI();
    }
}