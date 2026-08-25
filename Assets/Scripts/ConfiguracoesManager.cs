using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ConfiguracoesManager : MonoBehaviour
{
    [Header("Brilho")]
    public Slider sliderBrilho;

    [Header("Resolução")]
    public TMP_Dropdown dropdownResolucao;
    private Resolution[] resolucoes;

    [Header("Tela Cheia")]
    public Toggle toggleTelaCheia;

    void Start()
    {
        CarregarConfiguracoes();

        sliderBrilho.onValueChanged.AddListener(MudarBrilho);

        PopularResolucoes();
    }

    // ---------- BRILHO ----------
    public void MudarBrilho(float valor)
    {
        float proporcao = valor / sliderBrilho.maxValue;
        GameManager.Instance.currentData.brilho = proporcao;

        if (BrilhoOverlay.Instance != null)
            BrilhoOverlay.Instance.AtualizarBrilho(proporcao);
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

        // Remove duplicatas (mesma largura x altura em taxas de atualização diferentes)
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

        int indexSalvo = GameManager.Instance.currentData.resolucaoIndex;
        if (indexSalvo < 0 || indexSalvo >= resolucoes.Length)
            indexSalvo = resolucaoAtual;

        dropdownResolucao.SetValueWithoutNotify(indexSalvo);
        dropdownResolucao.RefreshShownValue();

        Resolution resSalva = resolucoes[indexSalvo];
        Screen.SetResolution(resSalva.width, resSalva.height, Screen.fullScreen);

        dropdownResolucao.onValueChanged.AddListener(MudarResolucao);
    }

    public void MudarResolucao(int index)
    {
        Resolution res = resolucoes[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        GameManager.Instance.currentData.resolucaoIndex = index;
    }

    // ---------- TELA CHEIA / JANELA ----------
    public void MudarTelaCheia(bool ativado)
    {
        Screen.fullScreen = ativado;
        GameManager.Instance.currentData.telaCheia = ativado;
    }

    // ---------- SALVAR TUDO ----------
    public void SalvarConfiguracoes()
    {
        GameManager.Instance.SaveGame();
    }

    // ---------- CARREGAR AO ABRIR ----------
    void CarregarConfiguracoes()
    {
        SaveData dados = GameManager.Instance.currentData;

        // dados.brilho é armazenado como proporção (0-1); convertemos de volta
        // para o range real do slider (que pode ir de 0 a 1, 0 a 100, etc.)
        float proporcao = dados.brilho;
        sliderBrilho.value = proporcao * sliderBrilho.maxValue;

        if (BrilhoOverlay.Instance != null)
            BrilhoOverlay.Instance.AtualizarBrilho(proporcao, instantaneo: true);

        toggleTelaCheia.isOn = dados.telaCheia;
        Screen.fullScreen = dados.telaCheia;

        toggleTelaCheia.onValueChanged.AddListener(MudarTelaCheia);
    }
}