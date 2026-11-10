using UnityEngine;
using UnityEngine.UI;

public class NavegacaoPaineis : MonoBehaviour
{
    [Header("Paineis em ordem (da esquerda pra direita)")]
    public GameObject[] paineis;

    [Header("Botoes de seta")]
    public Button botaoEsquerda;
    public Button botaoDireita;

    [Header("Comportamento")]
    public bool loop = false; // true = do ultimo painel volta pro primeiro (e vice-versa)

    private int indiceAtual = 0;

    void Start()
    {
        botaoEsquerda.onClick.AddListener(Anterior);
        botaoDireita.onClick.AddListener(Proximo);

        MostrarPainel(indiceAtual);
    }

    public void Proximo()
    {
        if (paineis == null || paineis.Length == 0) return;

        int novoIndice = indiceAtual + 1;

        if (novoIndice >= paineis.Length)
        {
            if (!loop) return;
            novoIndice = 0;
        }

        MostrarPainel(novoIndice);
    }

    public void Anterior()
    {
        if (paineis == null || paineis.Length == 0) return;

        int novoIndice = indiceAtual - 1;

        if (novoIndice < 0)
        {
            if (!loop) return;
            novoIndice = paineis.Length - 1;
        }

        MostrarPainel(novoIndice);
    }

    // Descobre qual painel esta ativo agora e atualiza o indice e as setas
    public void SincronizarComPainelAtivo()
    {
        if (paineis == null) return;

        for (int i = 0; i < paineis.Length; i++)
        {
            if (paineis[i] != null && paineis[i].activeSelf)
            {
                indiceAtual = i;
                break;
            }
        }

        AtualizarBotoes();
    }

    void MostrarPainel(int indice)
    {
        for (int i = 0; i < paineis.Length; i++)
        {
            if (paineis[i] != null)
                paineis[i].SetActive(i == indice);
        }

        indiceAtual = indice;
        AtualizarBotoes();
    }

    // Desativa visualmente a seta quando nao ha pra onde ir (so relevante se loop = false)
    void AtualizarBotoes()
    {
        if (loop) return;

        botaoEsquerda.interactable = indiceAtual > 0;
        botaoDireita.interactable = indiceAtual < paineis.Length - 1;
    }
}