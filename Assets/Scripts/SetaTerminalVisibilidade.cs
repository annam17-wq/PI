using UnityEngine;

// Colocar na SetaTerminal.
// Esconde a seta (e bloqueia o clique) enquanto:
//  - o zoom estiver aberto (terminal ou objeto ampliado), ou
//  - algum dos painéis da lista estiver ativo (ex.: Vis5 = casa de bonecas).
[RequireComponent(typeof(CanvasGroup))]
public class SetaTerminalVisibilidade : MonoBehaviour
{
    [Header("Painéis onde a seta do terminal NÃO pode aparecer (ex.: Vis5)")]
    public GameObject[] painelsOndeEscondeSeta;

    CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        bool esconder = ZoomManager.EstaAberto;

        if (!esconder && painelsOndeEscondeSeta != null)
        {
            foreach (GameObject p in painelsOndeEscondeSeta)
            {
                if (p != null && p.activeInHierarchy)
                {
                    esconder = true;
                    break;
                }
            }
        }

        cg.alpha = esconder ? 0f : 1f;
        cg.interactable = !esconder;
        cg.blocksRaycasts = !esconder;
    }
}