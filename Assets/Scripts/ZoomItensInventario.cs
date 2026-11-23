using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Coloque este script no objeto "ZoomManager" (o mesmo que tem o script ZoomManager).
// Ao clicar no ícone de um item no inventário:
//  - abre o zoom com o PNG ampliado desse item (se tiver PNG)
//  - abre um dialogo junto (se tiver)
//  - dispara o evento "Ao Clicar" (se tiver)
public class ZoomItensInventario : MonoBehaviour
{
    public static ZoomItensInventario Instance { get; private set; }

    [System.Serializable]
    public class ZoomDoItem
    {
        [Tooltip("Mesmo itemId do ItemData (ex.: Cruzid)")]
        public string itemId;

        [Tooltip("PNG ampliado que aparece no zoom. Vazio = nao abre zoom")]
        public Sprite spriteAmpliado;

        [Tooltip("Opcional: dialogo que abre junto com o zoom (ex.: DialogoCruzPerto). Vazio = sem dialogo")]
        public DialogoData dialogo;

        [Tooltip("Opcional: roda ao clicar no item (ex.: NarracaoFinal.Iniciar)")]
        public UnityEvent aoClicar;
    }

    [Header("Itens que reagem ao clique no inventário")]
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
        if (zoom == null) return;

        if (zoom.aoClicar != null) zoom.aoClicar.Invoke();

        if (zoom.spriteAmpliado == null) return;

        ZoomManager.Instance.AbrirImagem(zoom.spriteAmpliado);

        if (zoom.dialogo != null && DialogoManager.Instance != null)
            DialogoManager.Instance.IniciarDialogo(zoom.dialogo);
    }
}