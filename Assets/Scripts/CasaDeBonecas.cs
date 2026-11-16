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

    [Header("Quando completar a ordem")]
    public UnityEvent aoConcluir;

    private int etapa = 0;
    private Lanterna lanternaAtual = Lanterna.Nenhuma;

    public bool Concluido
    {
        get { return sequencia != null && etapa >= sequencia.Length; }
    }

    // etapas 0-1 = esquerda, 2-3 = cima, 4-5 = direita
    private Lanterna LanternaDaEtapa(int e)
    {
        return (Lanterna)(e / 2 + 1);
    }

    public void AlternarLanterna(Lanterna l)
    {
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
        if (sequencia == null || Concluido) return false;

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

        if (Concluido)
        {
            Debug.Log("Casa de bonecas: ordem completa!");
            if (aoConcluir != null) aoConcluir.Invoke();
        }
    }
}