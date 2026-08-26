using UnityEngine;

[CreateAssetMenu(fileName = "NovoItem", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;
    public string nomeExibido;
    public Sprite icone;
}