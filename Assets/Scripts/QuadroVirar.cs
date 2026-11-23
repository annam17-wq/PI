using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque no objeto "Quadro" (Vis3).
// Fica sem virar ate ser liberado (chame Liberar() em "Ao Concluir" da CasaDeBonecas).
// Antes de liberado, clicar dispara um dialogo. Depois de liberado, clicar troca o sprite pelo quadro de costas.
public class QuadroVirar : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Sprite do quadro de costas")]
    public Sprite spriteCostas;

    [Tooltip("Ligado: ja pode ser clicado. Deixe desligado; o enigma da casa de bonecas liga")]
    public bool liberado = false;

    [Header("Dialogos (use gatilhos SoManual em objetos ativos na raiz)")]
    [Tooltip("Dispara ao clicar no quadro ANTES de resolver o enigma")]
    public DialogoTrigger dialogoAntes;

    [Tooltip("Dispara quando o quadro vira")]
    public DialogoTrigger dialogoAoVirar;

    private bool virou;
    private bool dialogoAntesRodando;

    void Awake()
    {
        // Libera o clique de novo quando o dialogo termina (evita abrir varias vezes por spam de clique)
        if (dialogoAntes != null)
            dialogoAntes.aoTerminar.AddListener(() => dialogoAntesRodando = false);
    }

    // Ligue no "Ao Concluir ()" da CasaDeBonecas
    public void Liberar()
    {
        liberado = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (virou) return;

        // Enigma ainda nao resolvido: so fala
        if (!liberado)
        {
            if (dialogoAntes != null && !dialogoAntesRodando)
            {
                dialogoAntesRodando = true;
                dialogoAntes.Disparar();
            }
            return;
        }

        if (spriteCostas == null)
        {
            Debug.LogWarning($"[{name}] Falta o Sprite Costas!", this);
            return;
        }

        bool trocou = false;

        Image imagemUI = GetComponent<Image>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (imagemUI != null)
        {
            imagemUI.sprite = spriteCostas;
            trocou = true;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteCostas;
            trocou = true;
        }

        if (!trocou)
        {
            Debug.LogError($"[{name}] Precisa de Image ou SpriteRenderer!", this);
            return;
        }

        virou = true;

        if (dialogoAoVirar != null)
            dialogoAoVirar.Disparar();
    }
}