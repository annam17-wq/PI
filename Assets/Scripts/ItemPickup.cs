using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour,
    IPointerClickHandler, IDropHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ModoInteracao
    {
        ColetarEDesaparecer,
        TrocarSpriteAoClicar,
        TrocarSpriteAoReceberItem,
        ApenasArrastar // ícones do inventário: só servem para ser arrastados
    }

    [Header("Modo")]
    public ModoInteracao modo = ModoInteracao.ColetarEDesaparecer;

    [Header("Identificação")]
    [Tooltip("Coletar: igual ao itemId do ItemData. Arrastar: id que este objeto representa (ex.: Facaid)")]
    public string itemId;

    [Header("Arrastar")]
    [Tooltip("Marque nos objetos que o jogador pode arrastar até outros")]
    public bool arrastavel = false;

    [Tooltip("Se soltar em lugar errado, volta para onde estava")]
    public bool voltarSeNaoUsado = true;

    [Tooltip("Só para ícones do inventário (modo Apenas Arrastar): usado para descobrir qual item está no slot pelo ícone")]
    public ItemDatabase database;

    [Header("Condição")]
    [Tooltip("Ao clicar: precisa estar no inventário. Ao receber: precisa ser o item arrastado. Vazio = qualquer/nenhum")]
    public string itemRequerido;

    [Tooltip("Itens requeridos ADICIONAIS (além do 'Item Requerido' acima)")]
    public string[] itensRequeridosExtras;

    [Tooltip("Ligado: precisa entregar TODOS os itens (um por vez, em qualquer ordem). Desligado: qualquer um deles já basta")]
    public bool exigirTodosOsItens = true;

    [Tooltip("Consome o item requerido depois de usar")]
    public bool consumirItemRequerido = false;

    [Tooltip("Impede repetir a interação depois da primeira vez")]
    public bool trocarApenasUmaVez = true;

    [Header("Resultado: Trocar Sprite")]
    public bool trocarSprite = true;

    [Tooltip("Sprite que vai substituir o atual (ex.: urso sem cabeça)")]
    public Sprite spriteNovo;

    [Header("Resultado: Dar Item")]
    [Tooltip("Adiciona um item ao inventário quando a interação der certo")]
    public bool darItemAoUsar = false;

    [Tooltip("itemId do item que será adicionado (precisa existir no ItemDatabase)")]
    public string itemRecompensaId;

    [Header("Segunda etapa (clique depois da primeira troca)")]
    [Tooltip("Depois da 1ª troca, clicar no objeto faz a 2ª troca (modo Trocar Sprite Ao Receber Item)")]
    public bool segundaEtapaAoClicar = false;

    [Tooltip("Ligado: a 2ª etapa só acontece se o jogador ARRASTAR o item até o objeto ou, ao CLICAR, tiver o item no inventário")]
    public bool segundaEtapaExigeItem = false;

    [Tooltip("Item requerido na 2ª etapa")]
    public string itemRequerido2;

    [Tooltip("Itens requeridos ADICIONAIS na 2ª etapa (além do 'Item Requerido 2')")]
    public string[] itensRequeridos2Extras;

    [Tooltip("Ligado: precisa de TODOS os itens da 2ª etapa. Desligado: qualquer um deles já basta")]
    public bool exigirTodosOsItens2 = true;

    [Tooltip("Consome o(s) item(ns) requerido(s) da 2ª etapa")]
    public bool consumirItemRequerido2 = false;

    [Tooltip("Sprite que aparece na 2ª troca")]
    public Sprite spriteNovo2;

    [Tooltip("Dá um item ao inventário na 2ª etapa")]
    public bool darItemNaSegundaEtapa = false;

    [Tooltip("itemId do item dado na 2ª etapa (precisa existir no ItemDatabase)")]
    public string itemRecompensa2Id;

    [Header("Transparência")]
    [Tooltip("Ao interagir com sucesso, o objeto fica 100% visível (alpha = 1). Útil para objetos que começam invisíveis")]
    public bool tornarOpacoAoInteragir = false;

    [Header("Requisito de outro objeto")]
    [Tooltip("Este objeto só funciona depois que o objeto indicado já foi usado com sucesso (ex.: o 'lugar vara' só funciona depois do 'lugar escada')")]
    public ItemPickup precisaTerUsado;

    // Outros objetos consultam isto para saber se a interação já aconteceu
    public bool InteracaoConcluida => jaTrocou;

    [Header("Limite de usos")]
    [Tooltip("Depois de usado X vezes, o objeto desaparece. Quando ligado, ignora 'Trocar Apenas Uma Vez'")]
    public bool limitarUsos = false;

    [Tooltip("Quantas vezes o objeto pode ser usado antes de sumir")]
    [Min(1)] public int quantidadeDeUsos = 1;

    [Tooltip("Com 'Consumir Item Requerido' ligado: o item só sai do inventário no ÚLTIMO uso (ex.: a vara some depois do 2º peixe). Desligado: sai já no 1º uso. Não vale quando há vários itens requeridos por arrasto")]
    public bool consumirSoNoUltimoUso = false;

    [Header("Esconder outros objetos")]
    [Tooltip("Objetos da cena que somem quando a interação der certo (ex.: os peixes pescados)")]
    public GameObject[] objetosParaEsconder;

    [Tooltip("Desligado: cada uso esconde o PRÓXIMO da lista (uso 1 = elemento 0, uso 2 = elemento 1...). Ligado: o primeiro uso esconde todos")]
    public bool esconderTodosDeUmaVez = false;

    [Header("Mostrar outros objetos")]
    [Tooltip("Objetos que começam DESATIVADOS na cena e aparecem quando a interação der certo (ex.: a pá e a vara dentro do baú aberto)")]
    public GameObject[] objetosParaMostrar;

    // Troca de sprite
    private int usosFeitos;
    private bool consumiuNoUltimoResultado;
    private bool jaTrocou;
    private bool jaTrocou2;
    private SpriteRenderer spriteRenderer;
    private Image imagemUI;
    private RawImage rawImageUI;

    // Entregas parciais quando há vários itens requeridos (não é salvo)
    private readonly HashSet<string> itensEntregues = new HashSet<string>();
    private readonly HashSet<string> itensEntregues2 = new HashSet<string>();

    // Arrastar
    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Collider2D col2D;
    private Camera cam;
    private Transform paiOriginal;
    private int indiceOriginal;
    private Vector3 posicaoOriginal;
    private float zTela;
    private bool foiConsumido;
    private bool arrastando;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        imagemUI = GetComponent<Image>();
        rawImageUI = GetComponent<RawImage>();
        rect = transform as RectTransform;
        canvas = GetComponentInParent<Canvas>();
        col2D = GetComponent<Collider2D>();
        cam = Camera.main;

        // No UI, o CanvasGroup evita que o item arrastado bloqueie o raycast do alvo
        GarantirCanvasGroup();
    }

    // Cria o CanvasGroup se for um objeto de UI arrastável.
    // Também é chamado no início do arrasto, porque os ícones do inventário
    // têm 'arrastavel' ligado por código DEPOIS do Awake.
    private void GarantirCanvasGroup()
    {
        if (canvasGroup != null) return;
        if (!arrastavel) return;
        if (GetComponent<Graphic>() == null) return;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError($"[{gameObject.name}] GameManager.Instance é null!", this);
            enabled = false;
            return;
        }

        if (GameManager.Instance.currentData == null)
        {
            Debug.LogError($"[{gameObject.name}] currentData está null!", this);
            enabled = false;
            return;
        }

        if (GameManager.Instance.currentData.inventory == null)
        {
            Debug.LogError($"[{gameObject.name}] inventory está null!", this);
            enabled = false;
            return;
        }

        if (modo == ModoInteracao.ColetarEDesaparecer && ItemEstaNoInventario(itemId))
            gameObject.SetActive(false);
    }

    // Junta o itemRequerido com os extras numa lista só (1ª etapa)
    private List<string> ObterRequeridos()
    {
        return MontarLista(itemRequerido, itensRequeridosExtras);
    }

    // Idem, para a 2ª etapa
    private List<string> ObterRequeridos2()
    {
        return MontarLista(itemRequerido2, itensRequeridos2Extras);
    }

    private static List<string> MontarLista(string principal, string[] extras)
    {
        var lista = new List<string>();
        if (!string.IsNullOrEmpty(principal)) lista.Add(principal);

        if (extras != null)
        {
            foreach (string id in extras)
                if (!string.IsNullOrEmpty(id) && !lista.Contains(id))
                    lista.Add(id);
        }

        return lista;
    }

    // Confere se o inventário tem os itens exigidos. Devolve em 'usados' o que deve ser consumido.
    private bool ConferirNoInventario(List<string> requeridos, bool exigirTodos, out string[] usados)
    {
        usados = System.Array.Empty<string>();
        if (requeridos.Count == 0) return true;

        if (exigirTodos)
        {
            if (!requeridos.TrueForAll(ItemEstaNoInventario)) return false;
            usados = requeridos.ToArray();
            return true;
        }

        string achado = requeridos.Find(ItemEstaNoInventario);
        if (achado == null) return false;

        usados = new[] { achado };
        return true;
    }

    // ---------- Clique ----------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance == null) return;

        switch (modo)
        {
            case ModoInteracao.ColetarEDesaparecer:
                if (GameManager.Instance.AddItem(itemId))
                {
                    Debug.Log($"Item coletado: {itemId}");
                    gameObject.SetActive(false);
                }
                break;

            case ModoInteracao.TrocarSpriteAoClicar:
                {
                    if (!ConferirNoInventario(ObterRequeridos(), exigirTodosOsItens, out string[] usados))
                    {
                        Debug.Log("Faltam itens necessários no inventário.");
                        return;
                    }

                    ExecutarResultado(usados);
                    break;
                }

            case ModoInteracao.TrocarSpriteAoReceberItem:
                {
                    // Só funciona depois que a primeira troca já aconteceu
                    if (!segundaEtapaAoClicar || !jaTrocou || jaTrocou2) return;

                    string[] usados2 = System.Array.Empty<string>();

                    if (segundaEtapaExigeItem)
                    {
                        // O que já foi entregue por arrasto não precisa estar no inventário de novo
                        List<string> requeridos2 = ObterRequeridos2();
                        requeridos2.RemoveAll(id => itensEntregues2.Contains(id));

                        if (!ConferirNoInventario(requeridos2, exigirTodosOsItens2, out usados2))
                        {
                            Debug.Log("Faltam itens necessários para a 2ª etapa.");
                            return;
                        }
                    }

                    if (ExecutarSegundaEtapa(usados2))
                        itensEntregues2.Clear();
                    break;
                }
        }
    }

    // ---------- Este objeto recebe um item arrastado ----------
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"[{name}] OnDrop recebido. modo={modo}, pointerDrag={eventData.pointerDrag}");

        if (modo != ModoInteracao.TrocarSpriteAoReceberItem) return;
        if (eventData.pointerDrag == null) return;

        ItemPickup item = eventData.pointerDrag.GetComponent<ItemPickup>();
        if (item == null || !item.arrastavel)
        {
            Debug.Log("O objeto solto não tem ItemPickup arrastável.");
            return;
        }

        // Já fez a 1ª troca e a 2ª etapa exige item: o item solto vale para a 2ª etapa
        if (segundaEtapaAoClicar && segundaEtapaExigeItem && jaTrocou && !jaTrocou2)
        {
            ReceberItemSegundaEtapa(item);
            return;
        }

        List<string> requeridos = ObterRequeridos();

        // Item errado: não faz nada (ele volta sozinho para o lugar)
        if (requeridos.Count > 0 && !requeridos.Contains(item.itemId))
        {
            Debug.Log($"Item errado: '{item.itemId}'");
            return;
        }

        bool precisaDeVarios = exigirTodosOsItens && requeridos.Count > 1;

        // Caso simples: 0 ou 1 item, ou "qualquer um"
        if (!precisaDeVarios)
        {
            if (ExecutarResultado(new[] { item.itemId }) && consumiuNoUltimoResultado)
                item.foiConsumido = true;
            return;
        }

        // Vários itens: guarda cada entrega até completar
        if (!RequisitoCumprido()) return;

        if (limitarUsos)
        {
            if (usosFeitos >= quantidadeDeUsos) return;
        }
        else if (trocarApenasUmaVez && jaTrocou)
        {
            return;
        }

        if (itensEntregues.Contains(item.itemId))
        {
            Debug.Log($"'{item.itemId}' já foi entregue aqui.");
            return;
        }

        itensEntregues.Add(item.itemId);

        if (consumirItemRequerido)
        {
            RemoverDoInventario(item.itemId);
            item.foiConsumido = true;
        }

        if (itensEntregues.Count < requeridos.Count)
        {
            Debug.Log($"Item '{item.itemId}' entregue ({itensEntregues.Count}/{requeridos.Count}).");
            return;
        }

        // Todos entregues: dispara o efeito (os itens já foram consumidos acima)
        if (ExecutarResultado(System.Array.Empty<string>()))
            itensEntregues.Clear();
    }

    // Item arrastado até o objeto durante a 2ª etapa
    private void ReceberItemSegundaEtapa(ItemPickup item)
    {
        List<string> requeridos = ObterRequeridos2();

        if (requeridos.Count > 0 && !requeridos.Contains(item.itemId))
        {
            Debug.Log($"Item errado para a 2ª etapa: '{item.itemId}'");
            return;
        }

        bool precisaDeVarios = exigirTodosOsItens2 && requeridos.Count > 1;

        // Caso simples: 0 ou 1 item, ou "qualquer um"
        if (!precisaDeVarios)
        {
            if (ExecutarSegundaEtapa(new[] { item.itemId }) && consumirItemRequerido2)
                item.foiConsumido = true;
            return;
        }

        // Vários itens: guarda cada entrega até completar
        if (!RequisitoCumprido()) return;

        if (itensEntregues2.Contains(item.itemId))
        {
            Debug.Log($"'{item.itemId}' já foi entregue na 2ª etapa.");
            return;
        }

        bool ultimo = itensEntregues2.Count + 1 >= requeridos.Count;

        if (!ultimo)
        {
            itensEntregues2.Add(item.itemId);

            if (consumirItemRequerido2)
            {
                RemoverDoInventario(item.itemId);
                item.foiConsumido = true;
            }

            Debug.Log($"2ª etapa: '{item.itemId}' entregue ({itensEntregues2.Count}/{requeridos.Count}).");
            return;
        }

        // Último item: se a 2ª etapa falhar (ex.: inventário cheio), este item não é gasto
        if (ExecutarSegundaEtapa(new[] { item.itemId }))
        {
            itensEntregues2.Clear();
            if (consumirItemRequerido2) item.foiConsumido = true;
        }
    }

    // ---------- Este objeto é arrastado ----------
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"[{name}] OnBeginDrag. arrastavel={arrastavel}, modo={modo}");

        if (!arrastavel) return;

        if (modo == ModoInteracao.ApenasArrastar)
        {
            ResolverIdPeloIcone();

            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogWarning(
                    $"[{name}] Não achei o item deste slot. " +
                    "Confira o campo Database e se o 'icone' do ItemData é o mesmo sprite do slot.", this);
                return;
            }
        }

        // Garante que o item arrastado não bloqueie o raycast do alvo
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        GarantirCanvasGroup();

        arrastando = true;
        foiConsumido = false;
        paiOriginal = transform.parent;
        indiceOriginal = transform.GetSiblingIndex();
        posicaoOriginal = transform.position;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            if (canvas != null) transform.SetParent(canvas.rootCanvas.transform, true);
            transform.SetAsLastSibling();
        }
        else
        {
            if (col2D != null) col2D.enabled = false;
            if (cam != null) zTela = cam.WorldToScreenPoint(transform.position).z;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!arrastando) return;

        if (canvasGroup != null && rect != null)
        {
            Camera camCanvas = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera : null;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    rect, eventData.position, camCanvas, out Vector3 pos))
            {
                transform.position = pos;
            }
        }
        else if (cam != null)
        {
            Vector3 p = new Vector3(eventData.position.x, eventData.position.y, zTela);
            transform.position = cam.ScreenToWorldPoint(p);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!arrastando) return;
        arrastando = false;

        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;
        if (col2D != null) col2D.enabled = true;

        // Se foi consumido, o objeto some; senão volta (se configurado)
        if (foiConsumido || voltarSeNaoUsado)
        {
            if (canvasGroup != null)
            {
                transform.SetParent(paiOriginal, true);
                transform.SetSiblingIndex(indiceOriginal);
            }
            transform.position = posicaoOriginal;
        }

        if (foiConsumido)
        {
            if (modo == ModoInteracao.ApenasArrastar && imagemUI != null)
            {
                // Esvazia o slot visualmente, igual aos slots vazios
                imagemUI.enabled = false;
                itemId = "";

                // Redesenha o inventário a partir dos dados: se um item novo caiu neste
                // mesmo slot (ex.: o peixe no lugar da vara), ele reaparece em vez de ficar escondido
                if (InventoryUI.Instance != null) InventoryUI.Instance.AtualizarUI();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    // ---------- Resultado da interação ----------
    // idsUsados = itens que foram usados (arrastados ou exigidos no inventário)
    private bool ExecutarResultado(string[] idsUsados)
    {
        if (!RequisitoCumprido()) return false;

        if (limitarUsos)
        {
            if (usosFeitos >= quantidadeDeUsos) return false;
        }
        else if (trocarApenasUmaVez && jaTrocou)
        {
            return false;
        }

        if (trocarSprite && !AplicarSprite(spriteNovo)) return false;

        if (tornarOpacoAoInteragir) TornarOpaco();

        // Consome primeiro, para liberar espaço antes de dar o novo item
        consumiuNoUltimoResultado = DeveConsumirNesteUso();
        if (consumiuNoUltimoResultado && idsUsados != null)
        {
            foreach (string id in idsUsados)
                RemoverDoInventario(id);
        }

        if (darItemAoUsar)
            DarItemRecompensa();

        jaTrocou = true;
        usosFeitos++;

        EsconderObjetosDoUso();
        MostrarObjetos();

        // Com segunda etapa, o objeto só some depois dela
        if (!segundaEtapaAoClicar) VerificarLimiteDeUsos();

        return true;
    }

    // Decide se o item requerido sai do inventário neste uso (chamado antes de contar o uso)
    private bool DeveConsumirNesteUso()
    {
        if (!consumirItemRequerido) return false;

        if (limitarUsos && consumirSoNoUltimoUso)
            return usosFeitos + 1 >= quantidadeDeUsos;

        return true;
    }

    // Verifica se o objeto exigido já foi usado (sem exigência = sempre liberado)
    private bool RequisitoCumprido()
    {
        if (precisaTerUsado == null) return true;

        if (!precisaTerUsado.InteracaoConcluida)
        {
            Debug.Log($"[{gameObject.name}] ainda não liberado: precisa usar '{precisaTerUsado.name}' primeiro.");
            return false;
        }

        return true;
    }

    // Esconde os objetos da lista (um por uso, ou todos de uma vez)
    private void EsconderObjetosDoUso()
    {
        if (objetosParaEsconder == null || objetosParaEsconder.Length == 0) return;

        if (esconderTodosDeUmaVez)
        {
            foreach (GameObject obj in objetosParaEsconder)
                if (obj != null) obj.SetActive(false);
            return;
        }

        int indice = usosFeitos - 1;
        if (indice >= 0 && indice < objetosParaEsconder.Length && objetosParaEsconder[indice] != null)
            objetosParaEsconder[indice].SetActive(false);
    }

    // Ativa os objetos da lista (que começam desativados na cena)
    private void MostrarObjetos()
    {
        if (objetosParaMostrar == null) return;

        foreach (GameObject obj in objetosParaMostrar)
            if (obj != null) obj.SetActive(true);
    }

    // Some quando o número de usos chega ao limite
    private void VerificarLimiteDeUsos()
    {
        if (limitarUsos && usosFeitos >= quantidadeDeUsos)
        {
            Debug.Log($"[{gameObject.name}] atingiu {quantidadeDeUsos} uso(s) e desapareceu.");
            gameObject.SetActive(false);
        }
    }

    // ---------- Segunda etapa ----------
    // idsUsados = itens requeridos da 2ª etapa que serão consumidos agora (se 'consumirItemRequerido2')
    private bool ExecutarSegundaEtapa(string[] idsUsados)
    {
        if (!RequisitoCumprido()) return false;

        // Consome primeiro, para liberar espaço antes de dar o novo item
        bool consome = consumirItemRequerido2 && idsUsados != null && idsUsados.Length > 0;
        if (consome)
        {
            foreach (string id in idsUsados)
                RemoverDoInventario(id);
        }

        // Dá o item: se não der (ex.: inventário cheio), devolve o que foi consumido e nada muda
        if (darItemNaSegundaEtapa)
        {
            bool deu = false;

            if (string.IsNullOrEmpty(itemRecompensa2Id))
                Debug.LogWarning($"[{gameObject.name}] itemRecompensa2Id está vazio!", this);
            else if (GameManager.Instance.AddItem(itemRecompensa2Id))
                deu = true;
            else
                Debug.LogWarning($"Não foi possível adicionar '{itemRecompensa2Id}' ao inventário.", this);

            if (!deu)
            {
                if (consome)
                {
                    foreach (string id in idsUsados)
                        GameManager.Instance.AddItem(id);
                }
                return false;
            }

            Debug.Log($"Item recebido: {itemRecompensa2Id}");
        }

        if (spriteNovo2 != null) AplicarSprite(spriteNovo2);

        if (tornarOpacoAoInteragir) TornarOpaco();

        jaTrocou2 = true;

        VerificarLimiteDeUsos();
        return true;
    }

    // Deixa o objeto 100% visível (alpha = 1), mantendo a cor atual
    private void TornarOpaco()
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        if (imagemUI != null)
        {
            Color c = imagemUI.color;
            c.a = 1f;
            imagemUI.color = c;
        }

        if (rawImageUI != null)
        {
            Color c = rawImageUI.color;
            c.a = 1f;
            rawImageUI.color = c;
        }

        // Se o objeto tiver um CanvasGroup (que também controla a transparência)
        CanvasGroup grupo = GetComponent<CanvasGroup>();
        if (grupo != null) grupo.alpha = 1f;
    }

    private bool AplicarSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning($"[{gameObject.name}] sprite não foi definido!", this);
            return false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else if (imagemUI != null)
        {
            imagemUI.sprite = sprite;
        }
        else if (rawImageUI != null)
        {
            // RawImage usa Texture + UV Rect em vez de Sprite
            Texture tex = sprite.texture;
            Rect r = sprite.textureRect;
            rawImageUI.texture = tex;
            rawImageUI.uvRect = new Rect(
                r.x / tex.width, r.y / tex.height,
                r.width / tex.width, r.height / tex.height);
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Precisa de SpriteRenderer, Image ou RawImage!", this);
            return false;
        }

        Debug.Log($"Sprite de {gameObject.name} trocado.");
        return true;
    }

    private void DarItemRecompensa()
    {
        if (string.IsNullOrEmpty(itemRecompensaId))
        {
            Debug.LogWarning($"[{gameObject.name}] itemRecompensaId está vazio!", this);
            return;
        }

        if (GameManager.Instance.AddItem(itemRecompensaId))
            Debug.Log($"Item recebido: {itemRecompensaId}");
        else
            Debug.LogWarning($"Não foi possível adicionar '{itemRecompensaId}' ao inventário.", this);
    }

    // Descobre qual item está no slot comparando o sprite do ícone com o ItemDatabase
    private void ResolverIdPeloIcone()
    {
        if (database == null || database.itens == null || imagemUI == null) return;

        ItemData data = database.itens.Find(i => i != null && i.icone == imagemUI.sprite);
        itemId = data != null ? data.itemId : "";
    }

    private bool ItemEstaNoInventario(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return GameManager.Instance.currentData.inventory
            .Exists(i => i != null && i.itemId == id);
    }

    private void RemoverDoInventario(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        // Esvazia só UM slot com esse id (o primeiro), em vez de esvaziar todos.
        // Assim, se o jogador tiver 2 peixes, só 1 é consumido.
        // O slot é esvaziado (não removido da lista) para o tamanho do inventário não mudar.
        foreach (var item in GameManager.Instance.currentData.inventory)
        {
            if (item != null && item.itemId == id)
            {
                item.itemId = "";
                break;
            }
        }
    }
}