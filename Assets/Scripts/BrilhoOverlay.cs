using UnityEngine;
using UnityEngine.UI;

public class BrilhoOverlay : MonoBehaviour
{
    public static BrilhoOverlay Instance;

    public Image painelBrilho;
    public float velocidadeTransicao = 3f;
    private float alphaAlvo;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ConfigurarOverlay();
    }

    void ConfigurarOverlay()
    {
        // Canvas em Overlay: cobre a janela inteira, independente da câmera/letterbox
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 999; // fica por cima de tudo
        }

        if (painelBrilho != null)
        {
            painelBrilho.raycastTarget = false;

            // Estica o Image para preencher o Canvas inteiro
            RectTransform rt = painelBrilho.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
        }
    }

    void Start()
    {
        AtualizarBrilho(GameManager.Instance.currentData.brilho, instantaneo: true);
    }

    void Update()
    {
        if (painelBrilho != null)
        {
            Color cor = painelBrilho.color;
            cor.a = Mathf.Lerp(cor.a, alphaAlvo, Time.deltaTime * velocidadeTransicao);
            painelBrilho.color = cor;
        }
    }

    public void AtualizarBrilho(float valor, bool instantaneo = false)
    {
        alphaAlvo = 1f - valor;

        if (instantaneo && painelBrilho != null)
        {
            Color cor = painelBrilho.color;
            cor.a = alphaAlvo;
            painelBrilho.color = cor;
        }
    }
}