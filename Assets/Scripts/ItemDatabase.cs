using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventario/Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> itens;

    public ItemData GetItem(string itemId)
    {
        return itens.FirstOrDefault(i => i.itemId == itemId);
    }
}