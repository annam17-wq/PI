using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogo : MonoBehaviour
{
    [Header("Caixa de di�logo")]
    public GameObject caixaDialogo;
    public TMP_Text texto;

    [Header("Rosto")]
    public Image rostoImagem;
    public Sprite[] rostos;            // arraste quantos sprites quiser
    public float tempoCadaSprite = 0.25f;

    [Header("Fala")]
    [TextArea(3, 5)]
    public string fala = "Nunca vi uma rua t�o despeda�ada... Onde eu nasci n�o � assim.";

    [Header("Velocidade")]
    public float velocidadeTexto = 0.05f;

    [Header("Tempo at� desaparecer")]
    public float tempoAteSumir = 3f;

    [Header("Som de digita��o")]
    public AudioSource audioSource;
    public AudioClip somTeclado;
    public float intervaloSom = 0.08f;

    private bool jaFalou = false;

    private void Start()
    {
        caixaDialogo.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player") && !jaFalou)
        {
            jaFalou = true;
            StartCoroutine(MostrarDialogo());
        }
    }

    IEnumerator MostrarDialogo()
    {
        caixaDialogo.SetActive(true);
        texto.text = "";

        int spriteAtual = 0;
        float contadorSprite = 0f;
        float contadorSom = 0f;

        if (rostoImagem != null && rostos.Length > 0)
            rostoImagem.sprite = rostos[0];

        foreach (char letra in fala)
        {
            texto.text += letra;

            // Som
            if (letra != ' ' && audioSource != null && somTeclado != null && contadorSom <= 0f)
            {
                audioSource.PlayOneShot(somTeclado);
                contadorSom = intervaloSom;
            }
            contadorSom -= velocidadeTexto;

            // Troca de sprite
            contadorSprite += velocidadeTexto;
            if (rostoImagem != null && rostos.Length > 1 && contadorSprite >= tempoCadaSprite)
            {
                contadorSprite = 0f;
                spriteAtual = (spriteAtual + 1) % rostos.Length;
                rostoImagem.sprite = rostos[spriteAtual];
            }

            yield return new WaitForSeconds(velocidadeTexto);
        }

        // Volta pro primeiro rosto
        if (rostoImagem != null && rostos.Length > 0)
            rostoImagem.sprite = rostos[0];

        yield return new WaitForSeconds(tempoAteSumir);
        caixaDialogo.SetActive(false);
    }
}