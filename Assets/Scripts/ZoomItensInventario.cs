using System.Collections.Generic;
using UnityEngine;

// Coloque este script no objeto "ZoomManager" (o mesmo que tem o script ZoomManager).
// Ao clicar no ícone de um item no inventário, abre o zoom com o PNG ampliado desse item.
// Opcional: cada item pode abrir um diálogo junto com o zoom.
public class ZoomItensInventario : MonoBehaviour
{
    public static ZoomItensInventario Instance { get; private set; }

    [System.Serializable]
    public class ZoomDoItem
    {
        [Tooltip("Mesmo itemId do ItemData (ex.: Cruzid)")]
        public string itemId;

        [Tooltip("PNG ampliado que aparece no zoom")]
        public Sprite spriteAmpliado;

        [Tooltip("Opcional: diálogo que abre junto com o zoom (ex.: DialogoCruzPerto). Vazio = sem diálogo")]
        public DialogoData dialogo;
    }

    [Header("Itens que abrem zoom ao clicar no inventário")]
    public List<ZoomDoItem> itens = new List<ZoomDoItem>();

    void Awake()
    {
        Instance = this;
    }

    public void Abrir(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (ZoomManager.Instance == null || ZoomManager.EstaAberto) return;

        ZoomDoItem zoom = itens.Find(i => i != null && i.itemId == id);
        if (zoom == null || zoom.spriteAmpliado == null) return;

        ZoomManager.Instance.AbrirImagem(zoom.spriteAmpliado);

        if (zoom.dialogo != null && DialogoManager.Instance != null)
            DialogoManager.Instance.IniciarDialogo(zoom.dialogo);
    }
}