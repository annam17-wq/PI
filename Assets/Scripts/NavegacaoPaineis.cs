using UnityEngine;
using UnityEngine.UI;

public class NavegacaoPaineis : MonoBehaviour
{
    [Header("Painéis em ordem (da esquerda pra direita)")]
    public GameObject[] paineis;

    [Header("Botões de seta")]
    public Button botaoEsquerda;
    public Button botaoDireita;

    [Header("Comportamento")]
    public bool loop = false; // true = do último painel volta pro primeiro (e vice-versa)

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
            if (!loop) return; // já está no último painel, ignora o clique
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
            if (!loop) return; // já está no primeiro painel, ignora o clique
            novoIndice = paineis.Length - 1;
        }

        MostrarPainel(novoIndice);
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

    // Desativa visualmente a seta quando não há pra onde ir (só relevante se loop = false)
    void AtualizarBotoes()
    {
        if (loop) return;

        botaoEsquerda.interactable = indiceAtual > 0;
        botaoDireita.interactable = indiceAtual < paineis.Length - 1;
    }
}