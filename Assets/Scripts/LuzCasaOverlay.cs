using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LuzCasaOverlay : MonoBehaviour
{
    public float velocidadeLuz = 4f;
    public float velocidadeFlash = 15f;
    public float duracaoFlash = 0.35f;

    private Image painel;
    private Color corAlvo = Color.clear;
    private Color corFlash;
    private float tempoFlash;

    void Awake()
    {
        painel = GetComponent<Image>();
        painel.raycastTarget = false;
        painel.color = Color.clear;
    }

    public void DefinirCor(Color cor)
    {
        corAlvo = cor;
    }

    public void Apagar()
    {
        Color c = corAlvo;
        c.a = 0f;
        corAlvo = c;
    }

    public void Piscar(Color cor)
    {
        corFlash = cor;
        tempoFlash = duracaoFlash;
    }

    void Update()
    {
        Color alvo = corAlvo;
        float vel = velocidadeLuz;

        if (tempoFlash > 0f)
        {
            tempoFlash -= Time.unscaledDeltaTime;
            alvo = corFlash;
            vel = velocidadeFlash;
        }

        Color atual = painel.color;

        // evita escurecer no meio do fade quando sai do transparente
        if (atual.a < 0.01f)
        {
            atual = alvo;
            atual.a = 0f;
        }

        painel.color = Color.Lerp(atual, alvo, Time.unscaledDeltaTime * vel);
    }
}