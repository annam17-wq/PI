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
        }

        if (painelBrilho != null)
            painelBrilho.raycastTarget = false;
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
        alphaAlvo = 1f - valor; // ajuste essa fórmula igual você já usava (considerando maxValue do slider)

        if (instantaneo && painelBrilho != null)
        {
            Color cor = painelBrilho.color;
            cor.a = alphaAlvo;
            painelBrilho.color = cor;
        }
    }
}