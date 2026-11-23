using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque no objeto "Quadro" (Vis3).
// Fica sem reagir ao clique ate ser liberado (chame Liberar() em "Ao Concluir" da CasaDeBonecas).
// Depois de liberado, clicar troca o sprite pelo quadro de costas.
public class QuadroVirar : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Sprite do quadro de costas")]
    public Sprite spriteCostas;

    [Tooltip("Ligado: ja pode ser clicado. Deixe desligado; o enigma da casa de bonecas liga")]
    public bool liberado = false;

    [Header("Opcional")]
    [Tooltip("Dispara quando o quadro vira (use um gatilho em um objeto ativo na raiz)")]
    public DialogoTrigger dialogoAoVirar;

    private bool virou;

    // Ligue no "Ao Concluir ()" da CasaDeBonecas
    public void Liberar()
    {
        liberado = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!liberado || virou) return;

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