using System.Collections.Generic;
using UnityEngine;

// Inventário 2 fica ATRÁS do inventário 1.
// Quando o inventário 1 está cheio e o mouse passa por cima dele, o inventário 1
// (e os objetos "acompanhantes", ex.: a seta direita) deslizam para a esquerda
// e revelam o inventário 2.
// Coloque este script no mesmo objeto de antes (InventarioExpansivel).
public class InventarioExpansivel : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Vazio = usa o InventoryUI da cena")]
    public InventoryUI inventoryUI;

    [Tooltip("O painel do inventário 2 (InventarioU2), com os slots extras dentro")]
    public RectTransform painelExtra;

    [Tooltip("O painel do inventário 1 (InventarioUI). Vazio = usa o pai do primeiro slot")]
    public RectTransform painel1;

    [Tooltip("Objetos que andam junto com o inventário 1 (ex.: a seta Direita)")]
    public RectTransform[] acompanhantes;

    [Header("Quando abrir")]
    [Tooltip("Só abre se TODOS os slots do inventário 1 estiverem ocupados (ou se já houver itens no 2)")]
    public bool soAbrirQuandoCheio = true;

    [Tooltip("Quando um item novo cai no inventário 2, abre sozinho por alguns segundos")]
    public bool abrirAoReceberItem = true;
    public float tempoAbertoAoReceber = 1.5f;

    [Tooltip("Tempo com o mouse fora antes de fechar")]
    public float atrasoParaFechar = 0.35f;

    [Header("Animação")]
    public float duracaoAnimacao = 0.25f;
    public AnimationCurve curva = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("Largura de cada painel preto quando os dois estão abertos. Fechado, voltam à largura normal. 0 = não encolhe")]
    public float larguraAberta = 220f;

    private RectTransform[] moveis;   // painel1 + acompanhantes
    private Vector2[] origem;         // posição fechada de cada um
    private Vector2[] deslocamento;   // quanto cada um anda (no espaço do próprio pai)

    private float larguraOrig1, larguraOrig2;       // larguras normais (fechado)
    private float larguraAberta1, larguraAberta2;   // larguras com os dois abertos
    private Vector2 origemExtra;                    // posição normal do painel 2
    private float ultimoProgresso;

    private Canvas canvasRaiz;
    private int tamanhoBase;
    private int quantidadeExtra;
    private float progresso;
    private bool aberto;
    private bool pronto;
    private bool avisouMotivo;
    private float tempoFora;
    private float manterAbertoAte;
    private int ultimaContagemExtra;

    private readonly Vector3[] cantos = new Vector3[4];

    void Start()
    {
        if (inventoryUI == null) inventoryUI = InventoryUI.Instance;

        if (inventoryUI == null || inventoryUI.slots == null || inventoryUI.slots.Count == 0)
        {
            Debug.LogError("[InventarioExpansivel] InventoryUI não encontrado ou sem slots.", this);
            enabled = false;
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.currentData == null)
        {
            Debug.LogError("[InventarioExpansivel] GameManager/currentData indisponível.", this);
            enabled = false;
            return;
        }

        if (painelExtra == null)
        {
            Debug.LogError("[InventarioExpansivel] Arraste o InventarioU2 para o campo 'Painel Extra'.", this);
            enabled = false;
            return;
        }

        // Separa os slots do inventário 1 (base) dos do inventário 2 (extras)
        List<InventorySlotUI> slotsBase = new List<InventorySlotUI>();
        foreach (InventorySlotUI s in inventoryUI.slots)
            if (s != null && !s.transform.IsChildOf(painelExtra)) slotsBase.Add(s);

        if (slotsBase.Count == 0)
        {
            Debug.LogError("[InventarioExpansivel] Nenhum slot do inventário 1 na lista do InventoryUI.", this);
            enabled = false;
            return;
        }

        if (painel1 == null) painel1 = slotsBase[0].transform.parent as RectTransform;

        Canvas canvasDoPainel = painel1 != null ? painel1.GetComponentInParent<Canvas>() : null;
        if (painel1 == null || canvasDoPainel == null)
        {
            Debug.LogError("[InventarioExpansivel] Os slots precisam estar dentro de um painel UI dentro de um Canvas.", this);
            enabled = false;
            return;
        }

        canvasRaiz = canvasDoPainel.rootCanvas;

        tamanhoBase = 0;
        foreach (InventorySlotUI s in slotsBase)
            if (s.index + 1 > tamanhoBase) tamanhoBase = s.index + 1;

        // Slots do inventário 2: recebem os índices na ordem da hierarquia (5, 6, 7...)
        InventorySlotUI[] extras = painelExtra.GetComponentsInChildren<InventorySlotUI>(true);
        for (int k = 0; k < extras.Length; k++)
            extras[k].index = tamanhoBase + k;

        // A ordem da lista precisa acompanhar o index
        List<InventorySlotUI> nova = new List<InventorySlotUI>(slotsBase);
        nova.AddRange(extras);
        inventoryUI.slots = nova;
        quantidadeExtra = extras.Length;

        int precisa = tamanhoBase + quantidadeExtra;
        if (GameManager.Instance.currentData.inventory.Count < precisa)
        {
            Debug.LogError($"[InventarioExpansivel] Os dados do inventário têm {GameManager.Instance.currentData.inventory.Count} posições, " +
                           $"mas são necessárias {precisa}. Aumente o tamanho da lista no GameManager/SaveData.", this);
            enabled = false;
            return;
        }

        Canvas.ForceUpdateCanvases();
        PrepararMovimento();

        ultimaContagemExtra = ContarItensNaExtensao();
        pronto = true;

        inventoryUI.AtualizarUI();
    }

    // Calcula o quanto o inventário 1 (e os acompanhantes) precisam andar para a esquerda
    private void PrepararMovimento()
    {
        larguraOrig1 = painel1.rect.width;
        larguraOrig2 = painelExtra.rect.width;
        larguraAberta1 = larguraAberta > 0f ? Mathf.Min(larguraAberta, larguraOrig1) : larguraOrig1;
        larguraAberta2 = larguraAberta > 0f ? Mathf.Min(larguraAberta, larguraOrig2) : larguraOrig2;

        painel1.GetWorldCorners(cantos);
        float direita1 = cantos[2].x;
        painelExtra.GetWorldCorners(cantos);
        float direita2 = cantos[2].x;
        float escala = larguraOrig2 > 0.01f ? (cantos[2].x - cantos[0].x) / larguraOrig2 : 1f;

        // Com o painel 2 já encolhido, a borda direita do 1 deve encostar na borda esquerda dele
        float distanciaMundo = Mathf.Max(0f, (direita1 - direita2) + larguraAberta2 * escala);
        Vector3 deltaMundo = new Vector3(-distanciaMundo, 0f, 0f);

        List<RectTransform> lista = new List<RectTransform> { painel1 };
        if (acompanhantes != null)
            foreach (RectTransform rt in acompanhantes)
                if (rt != null && rt != painel1) lista.Add(rt);

        moveis = lista.ToArray();
        origem = new Vector2[moveis.Length];
        deslocamento = new Vector2[moveis.Length];

        for (int i = 0; i < moveis.Length; i++)
        {
            origem[i] = moveis[i].anchoredPosition;
            Transform pai = moveis[i].parent;
            Vector3 local = pai != null ? pai.InverseTransformVector(deltaMundo) : deltaMundo;
            deslocamento[i] = new Vector2(local.x, local.y);
        }

        origemExtra = painelExtra.anchoredPosition;
    }

    // ---------- Lógica de abrir/fechar ----------
    void Update()
    {
        if (!pronto) return;

        Vector2 mouse = PosicaoMouse();
        bool apertando = BotaoPressionado();

        // Item novo caiu no inventário 2: abre sozinho por um instante
        int contagem = ContarItensNaExtensao();
        if (abrirAoReceberItem && contagem > ultimaContagemExtra)
            manterAbertoAte = Time.unscaledTime + tempoAbertoAoReceber;
        ultimaContagemExtra = contagem;

        bool forcarAberto = Time.unscaledTime < manterAbertoAte;

        Rect r1 = RetanguloNaTela(painel1);
        Rect r2 = RetanguloNaTela(painelExtra);

        if (!aberto)
        {
            if (r1.Contains(mouse))
            {
                if (PodeAbrir()) Abrir();
                else AvisarMotivo();
            }
            else
            {
                avisouMotivo = false;
            }

            if (forcarAberto) Abrir();
        }
        else
        {
            bool dentro = r1.Contains(mouse) || r2.Contains(mouse);

            if (dentro || apertando || forcarAberto)
            {
                tempoFora = 0f;
            }
            else
            {
                tempoFora += Time.unscaledDeltaTime;
                if (tempoFora >= atrasoParaFechar) Fechar();
            }
        }

        Animar();
    }

    public void Abrir()
    {
        aberto = true;
        tempoFora = 0f;
    }

    public void Fechar()
    {
        aberto = false;
    }

    // Para testar sem mouse: botão direito no título do componente
    [ContextMenu("Abrir por 3 segundos (teste)")]
    private void AbrirTeste()
    {
        manterAbertoAte = Time.unscaledTime + 3f;
    }

    private bool PodeAbrir()
    {
        if (!soAbrirQuandoCheio) return true;
        return BaseCheia() || ContarItensNaExtensao() > 0;
    }

    private void AvisarMotivo()
    {
        if (avisouMotivo) return;
        avisouMotivo = true;

        int ocupados = 0;
        for (int i = 0; i < tamanhoBase; i++)
            if (SlotTemItem(i)) ocupados++;

        Debug.Log($"[InventarioExpansivel] Não abriu: o inventário 1 não está cheio ({ocupados} de {tamanhoBase} slots). " +
                  "Encha os slots ou desmarque 'So Abrir Quando Cheio' para testar.");
    }

    // Time.unscaledDeltaTime faz funcionar mesmo com o jogo pausado
    private void Animar()
    {
        float alvo = aberto ? 1f : 0f;

        if (!Mathf.Approximately(progresso, alvo))
        {
            float passo = duracaoAnimacao > 0f ? Time.unscaledDeltaTime / duracaoAnimacao : 1f;
            progresso = Mathf.MoveTowards(progresso, alvo, passo);
        }

        if (Mathf.Approximately(progresso, ultimoProgresso)) return;
        ultimoProgresso = progresso;

        float t = curva.Evaluate(progresso);

        // Acompanhantes (ex.: seta) só deslizam
        for (int i = 1; i < moveis.Length; i++)
            moveis[i].anchoredPosition = origem[i] + deslocamento[i] * t;

        // Painéis pretos: deslizam (o 1) e encolhem mantendo a borda direita no lugar
        AplicarLargura(painel1, larguraOrig1, larguraAberta1, t, origem[0] + deslocamento[0] * t);
        AplicarLargura(painelExtra, larguraOrig2, larguraAberta2, t, origemExtra);
    }

    // Muda a largura do painel sem mexer na borda direita (encolhe para o lado esquerdo)
    private void AplicarLargura(RectTransform rt, float larguraNormal, float larguraFinal, float t, Vector2 posicaoNormal)
    {
        float w = Mathf.Lerp(larguraNormal, larguraFinal, t);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        rt.anchoredPosition = new Vector2(
            posicaoNormal.x + (1f - rt.pivot.x) * (larguraNormal - w),
            posicaoNormal.y);
    }

    // ---------- Dados do inventário ----------
    private bool SlotTemItem(int i)
    {
        List<InventoryItem> inv = GameManager.Instance.currentData.inventory;
        return i >= 0 && i < inv.Count && inv[i] != null && !string.IsNullOrEmpty(inv[i].itemId);
    }

    private bool BaseCheia()
    {
        for (int i = 0; i < tamanhoBase; i++)
            if (!SlotTemItem(i)) return false;
        return true;
    }

    private int ContarItensNaExtensao()
    {
        int n = 0;
        for (int i = tamanhoBase; i < tamanhoBase + quantidadeExtra; i++)
            if (SlotTemItem(i)) n++;
        return n;
    }

    // ---------- Geometria ----------
    private Rect RetanguloNaTela(RectTransform rt)
    {
        rt.GetWorldCorners(cantos);
        Camera cam = CameraDoCanvas();
        Vector2 a = RectTransformUtility.WorldToScreenPoint(cam, cantos[0]);
        Vector2 b = RectTransformUtility.WorldToScreenPoint(cam, cantos[2]);
        return Rect.MinMaxRect(
            Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y),
            Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
    }

    private Camera CameraDoCanvas()
    {
        if (canvasRaiz == null || canvasRaiz.renderMode == RenderMode.ScreenSpaceOverlay) return null;
        return canvasRaiz.worldCamera;
    }

    // ---------- Mouse ----------
    private static Vector2 PosicaoMouse()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var mouse = UnityEngine.InputSystem.Mouse.current;
        return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    private static bool BotaoPressionado()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var mouse = UnityEngine.InputSystem.Mouse.current;
        return mouse != null && mouse.leftButton.isPressed;
#else
        return Input.GetMouseButton(0);
#endif
    }
}