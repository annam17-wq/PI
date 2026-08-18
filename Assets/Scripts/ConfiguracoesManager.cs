using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ConfiguracoesManager : MonoBehaviour
{
    [Header("Brilho")]
    public Slider sliderBrilho;
    public Image painelBrilho;
    public float velocidadeTransicao = 3f; // ajuste a suavidade aqui

    private float alphaAlvo;

    [Header("Resolução")]
    public TMP_Dropdown dropdownResolucao;
    private Resolution[] resolucoes;

    [Header("Tela Cheia")]
    public Toggle toggleTelaCheia;

    void Start()
    {
        CarregarConfiguracoes();
        PopularResolucoes();
    }

    void Update()
    {
        // Suaviza a transição do alpha do painel de brilho
        if (painelBrilho != null)
        {
            Color cor = painelBrilho.color;
            cor.a = Mathf.Lerp(cor.a, alphaAlvo, Time.deltaTime * velocidadeTransicao);
            painelBrilho.color = cor;
        }
    }

    // ---------- BRILHO ----------
    public void MudarBrilho(float valor)
    {
        // valor vai de 0 (bem escuro) a 1 (sem escurecer)
        alphaAlvo = 1f - valor; // define o alvo, o Update() vai suavizar até chegar lá

        PlayerPrefs.SetFloat("Brilho", valor);
    }

    // ---------- RESOLUÇÃO ----------
    void PopularResolucoes()
    {
        resolucoes = Screen.resolutions;
        dropdownResolucao.ClearOptions();

        List<string> opcoes = new List<string>();
        int resolucaoAtual = 0;

        for (int i = 0; i < resolucoes.Length; i++)
        {
            string opcao = resolucoes[i].width + " x " + resolucoes[i].height;
            opcoes.Add(opcao);

            if (resolucoes[i].width == Screen.currentResolution.width &&
                resolucoes[i].height == Screen.currentResolution.height)
            {
                resolucaoAtual = i;
            }
        }

        dropdownResolucao.AddOptions(opcoes);
        dropdownResolucao.value = resolucaoAtual;
        dropdownResolucao.RefreshShownValue();
    }

    public void MudarResolucao(int index)
    {
        Resolution res = resolucoes[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolucaoIndex", index);
    }

    // ---------- TELA CHEIA / JANELA ----------
    public void MudarTelaCheia(bool ativado)
    {
        Screen.fullScreen = ativado;
        PlayerPrefs.SetInt("TelaCheia", ativado ? 1 : 0);
    }

    // ---------- SALVAR TUDO ----------
    public void SalvarConfiguracoes()
    {
        PlayerPrefs.Save();
    }

    // ---------- CARREGAR AO ABRIR ----------
    void CarregarConfiguracoes()
    {
        float brilho = PlayerPrefs.GetFloat("Brilho", 1f);
        sliderBrilho.value = brilho;
        alphaAlvo = 1f - brilho;

        // Aplica o alpha direto na primeira vez, sem transição (senão começa transparente e "acende")
        if (painelBrilho != null)
        {
            Color cor = painelBrilho.color;
            cor.a = alphaAlvo;
            painelBrilho.color = cor;
        }

        bool telaCheia = PlayerPrefs.GetInt("TelaCheia", 1) == 1;
        toggleTelaCheia.isOn = telaCheia;
        Screen.fullScreen = telaCheia;
    }
}