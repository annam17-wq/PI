using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque no objeto "Alavanca" (Vis5). Clicar alterna entre ligada (baixo) e desligada (cima).
// Enquanto estiver desligada, nada na casa de bonecas funciona.
[RequireComponent(typeof(Image))]
public class AlavancaCasa : MonoBehaviour, IPointerClickHandler
{
    [Header("Casa de bonecas")]
    public CasaDeBonecas casa;

    [Header("Visual")]
    [Tooltip("Sprite da alavanca para cima (desligada). Vazio = usa o sprite que ja esta na Image")]
    public Sprite spriteDesligada;

    [Tooltip("Sprite da alavanca para baixo (ligada). Vazio = a alavanca e virada de cabeca para baixo (escala Y invertida)")]
    public Sprite spriteLigada;

    [Header("Estado")]
    public bool ligadaNoInicio = false;

    public bool Ligada { get; private set; }

    private Image imagem;
    private Vector3 escalaOriginal;

    void Awake()
    {
        imagem = GetComponent<Image>();
        escalaOriginal = transform.localScale;

        if (spriteDesligada == null && imagem != null)
            spriteDesligada = imagem.sprite;
    }

    void Start()
    {
        Aplicar(ligadaNoInicio);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Depois que o enigma e concluido, a alavanca fica travada
        if (casa != null && casa.Concluido) return;

        Aplicar(!Ligada);

        // Ordem completa: puxar a alavanca de novo conclui o enigma
        if (casa != null && casa.SequenciaCompleta)
            casa.Concluir();
    }

    private void Aplicar(bool ligar)
    {
        Ligada = ligar;

        if (casa != null) casa.DefinirAtiva(ligar);
        else Debug.LogWarning($"[{name}] Falta arrastar a CasaDeBonecas no campo Casa.", this);

        if (imagem != null && spriteLigada != null)
        {
            imagem.sprite = ligar ? spriteLigada : spriteDesligada;
        }
        else
        {
            Vector3 escala = escalaOriginal;
            if (ligar) escala.y = -escala.y;
            transform.localScale = escala;
        }
    }
}