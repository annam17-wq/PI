using UnityEngine;
using UnityEngine.Events;

public enum Lanterna { Nenhuma, Esquerda, Cima, Direita }

public class CasaDeBonecas : MonoBehaviour
{
    [Header("Boneca")]
    public string idBoneca = "Bonecaid";

    [Header("Ordem: cama, TV, fogao, banheiro, estante, cama")]
    public LugarBoneca[] sequencia;

    [Header("Luz")]
    public LuzCasaOverlay luz;
    public Color corEsquerda = new Color(1f, 0.65f, 0.15f, 0.30f);   // laranja/amarelo
    public Color corCima = new Color(1f, 1f, 0.80f, 0.30f);      // amarelo/branco
    public Color corDireita = new Color(0.35f, 0.30f, 0.75f, 0.35f); // anoitecer roxo/azul
    public Color corAcerto = new Color(0.2f, 1f, 0.3f, 0.45f);
    public Color corErro = new Color(1f, 0.15f, 0.15f, 0.45f);

    [Header("Alavanca")]
    [Tooltip("Controlado pela AlavancaCasa. Desligado: nada na casa funciona (lanternas, soltar boneca, pegar boneca)")]
    [SerializeField] private bool ativa = false;

    [Header("Quando colocar a ultima boneca (ainda falta puxar a alavanca)")]
    public UnityEvent aoCompletarSequencia;

    [Header("Quando puxar a alavanca depois da ordem completa")]
    public UnityEvent aoConcluir;

    private int etapa = 0;
    private bool concluido = false;
    private Lanterna lanternaAtual = Lanterna.Nenhuma;

    public bool Ativa
    {
        get { return ativa; }
    }

    // Todas as bonecas colocadas na ordem certa (ainda falta puxar a alavanca)
    public bool SequenciaCompleta
    {
        get { return sequencia != null && etapa >= sequencia.Length; }
    }

    // Enigma concluido de verdade: sequencia completa + alavanca puxada de novo
    public bool Concluido
    {
        get { return concluido; }
    }

    // Chamado pela AlavancaCasa quando a alavanca e puxada com a sequencia completa
    public void Concluir()
    {
        if (concluido || !SequenciaCompleta) return;

        concluido = true;
        Debug.Log("Casa de bonecas: alavanca puxada, enigma concluido!");
        if (aoConcluir != null) aoConcluir.Invoke();
    }

    // Chamado pela AlavancaCasa
    public void DefinirAtiva(bool valor)
    {
        ativa = valor;

        // Alavanca desligada: apaga a luz e solta a lanterna. O progresso das bonecas e mantido
        if (!ativa)
        {
            lanternaAtual = Lanterna.Nenhuma;
            if (luz != null) luz.Apagar();
        }
    }

    // etapas 0-1 = esquerda, 2-3 = cima, 4-5 = direita
    private Lanterna LanternaDaEtapa(int e)
    {
        return (Lanterna)(e / 2 + 1);
    }

    public void AlternarLanterna(Lanterna l)
    {
        if (!ativa)
        {
            Debug.Log("Casa de bonecas desligada: ative a alavanca primeiro.");
            return;
        }

        lanternaAtual = (lanternaAtual == l) ? Lanterna.Nenhuma : l;

        if (luz == null) return;

        switch (lanternaAtual)
        {
            case Lanterna.Esquerda: luz.DefinirCor(corEsquerda); break;
            case Lanterna.Cima: luz.DefinirCor(corCima); break;
            case Lanterna.Direita: luz.DefinirCor(corDireita); break;
            default: luz.Apagar(); break;
        }
    }

    // Chamado quando a boneca e solta em qualquer lugar
    public bool TentarColocar(LugarBoneca lugar)
    {
        if (!ativa)
        {
            Debug.Log("Casa de bonecas desligada: ative a alavanca primeiro.");
            return false;
        }

        if (sequencia == null || SequenciaCompleta) return false;

        bool lugarCerto = sequencia[etapa] == lugar;
        bool luzCerta = lanternaAtual == LanternaDaEtapa(etapa);

        if (!lugarCerto || !luzCerta)
        {
            Debug.Log("Errado: lugar certo = " + lugarCerto + ", luz certa = " + luzCerta);
            if (luz != null) luz.Piscar(corErro);
            return false;
        }

        return true;
    }

    public void RegistrarColocacao()
    {
        etapa++;
        if (luz != null) luz.Piscar(corAcerto);

        if (SequenciaCompleta)
        {
            Debug.Log("Casa de bonecas: ordem completa! Puxe a alavanca para concluir.");
            if (aoCompletarSequencia != null) aoCompletarSequencia.Invoke();
        }
    }
}