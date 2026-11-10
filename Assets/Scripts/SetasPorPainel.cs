using UnityEngine;

public class SetasPorPainel : MonoBehaviour
{
    [Header("Painel da casa de bonecas")]
    public GameObject painelCasaBonecas; // Vis5

    [Header("Setas")]
    public GameObject setaEsquerda;
    public GameObject setaDireita;
    public GameObject setaBaixo;

    [Header("Navegacao")]
    public NavegacaoPaineis navegacao;

    private bool estavaNaCasa;

    void LateUpdate()
    {
        bool naCasa = painelCasaBonecas != null && painelCasaBonecas.activeInHierarchy;

        if (naCasa)
        {
            if (setaEsquerda != null) setaEsquerda.SetActive(false);
            if (setaDireita != null) setaDireita.SetActive(false);
            if (setaBaixo != null) setaBaixo.SetActive(true);
        }
        else
        {
            if (setaBaixo != null) setaBaixo.SetActive(false);

            // Ao sair da casa, devolve as setas uma unica vez
            if (estavaNaCasa)
            {
                if (setaEsquerda != null) setaEsquerda.SetActive(true);
                if (setaDireita != null) setaDireita.SetActive(true);
                if (navegacao != null) navegacao.SincronizarComPainelAtivo();
            }
        }

        estavaNaCasa = naCasa;
    }
}