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
        // Garante que o painel preto nunca intercepte cliques da UI
        if (painelBrilho != null)
            painelBrilho.raycastTarget = false;

        CarregarConfiguracoes();

        // Registrado ANTES de PopularResolucoes() para nunca ficar pra trás
        // caso algo ali (ex: dropdown não atribuído) gere erro
        sliderBrilho.onValueChanged.AddListener(MudarBrilho);

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
        // Proporcional ao range real do slider (funciona com Max Value = 1 ou = 100)
        // valor no máximo do slider = painel totalmente transparente (sem escurecer)
        // valor no mínimo do slider = painel totalmente opaco (tela bem escura)
        float proporcao = valor / sliderBrilho.maxValue;
        alphaAlvo = 1f - proporcao; // define o alvo, o Update() vai suavizar até chegar lá
        PlayerPrefs.SetFloat("Brilho", valor);
    }

    // ---------- RESOLUÇÃO ----------
    void PopularResolucoes()
    {
        if (dropdownResolucao == null)
        {
            Debug.LogWarning("Dropdown Resolucao não está atribuído no Inspector — pulando configuração de resolução.");
            return;
        }

        Resolution[] todasResolucoes = Screen.resolutions;

        // Remove duplicatas (o Unity lista a mesma largura x altura várias vezes,
        // uma para cada taxa de atualização suportada pelo monitor)
        List<Resolution> resolucoesFiltradas = new List<Resolution>();
        HashSet<string> vistas = new HashSet<string>();

        foreach (Resolution res in todasResolucoes)
        {
            string chave = res.width + "x" + res.height;
            if (!vistas.Contains(chave))
            {
                vistas.Add(chave);
                resolucoesFiltradas.Add(res);
            }
        }

        resolucoes = resolucoesFiltradas.ToArray();
        dropdownResolucao.ClearOptions();

        List<string> opcoes = new List<string>();
        int resolucaoAtual = 0;

        for (int i = 0; i < resolucoes.Length; i++)
        {
            opcoes.Add(resolucoes[i].width + " x " + resolucoes[i].height);

            if (resolucoes[i].width == Screen.currentResolution.width &&
                resolucoes[i].height == Screen.currentResolution.height)
            {
                resolucaoAtual = i;
            }
        }

        dropdownResolucao.AddOptions(opcoes);

        // Se já existe uma resolução salva anteriormente, usa ela; senão usa a atual da tela
        int indexSalvo = PlayerPrefs.GetInt("ResolucaoIndex", resolucaoAtual);
        if (indexSalvo < 0 || indexSalvo >= resolucoes.Length)
            indexSalvo = resolucaoAtual;

        dropdownResolucao.SetValueWithoutNotify(indexSalvo);
        dropdownResolucao.RefreshShownValue();

        // Aplica de fato a resolução salva (não só mostra no dropdown)
        Resolution resSalva = resolucoes[indexSalvo];
        Screen.SetResolution(resSalva.width, resSalva.height, Screen.fullScreen);

        dropdownResolucao.onValueChanged.AddListener(MudarResolucao);
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
        // Padrão: slider no máximo (sem salvamento prévio) = painel 100% transparente
        float brilho = PlayerPrefs.GetFloat("Brilho", sliderBrilho.maxValue);
        sliderBrilho.value = brilho;
        alphaAlvo = 1f - (brilho / sliderBrilho.maxValue);

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

        toggleTelaCheia.onValueChanged.AddListener(MudarTelaCheia);
    }
}