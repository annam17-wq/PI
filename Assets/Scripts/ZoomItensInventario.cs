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

        [Tooltip("Marque no item do cha: ao clicar, inicia a narracao final (acha o NarracaoFinal da cena sozinho, sem precisar ligar nada)")]
        public bool iniciaNarracaoFinal;

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
        Debug.Log("[ZoomItens] Clique no item do inventario: '" + id + "'");

        if (string.IsNullOrEmpty(id)) return;
        if (ZoomManager.Instance == null || ZoomManager.EstaAberto)
        {
            Debug.Log("[ZoomItens] Ignorado: zoom ja esta aberto ou ZoomManager nao existe");
            return;
        }

        ZoomDoItem zoom = itens.Find(i => i != null && i.itemId == id);
        if (zoom == null)
        {
            Debug.Log("[ZoomItens] Esse itemId nao esta na lista Itens do ZoomManager (confira se esta escrito igual)");
            return;
        }

        if (zoom.iniciaNarracaoFinal)
        {
            NarracaoFinal narracao = Object.FindFirstObjectByType<NarracaoFinal>(FindObjectsInactive.Include);
            if (narracao != null) narracao.Iniciar();
            else Debug.LogWarning("[ZoomItens] NarracaoFinal nao foi encontrada na cena");
        }

        if (zoom.aoClicar != null) zoom.aoClicar.Invoke();

        if (zoom.spriteAmpliado == null) return;

        ZoomManager.Instance.AbrirImagem(zoom.spriteAmpliado);

        if (zoom.dialogo != null && DialogoManager.Instance != null)
            DialogoManager.Instance.IniciarDialogo(zoom.dialogo);
    }
}