using UnityEngine;
using UnityEngine.UI;

// Colocar no objeto "ExtraPote" (o conteúdo extra do zoom, pai da parte clicável).
// Quando o item da parte clicável é coletado (ela some), o PNG ampliado troca por outro.
// Se o zoom for aberto de novo depois, o PNG novo já aparece.
public class ZoomTrocarAoColetar : MonoBehaviour
{
    [Header("A parte clicável que dá o item (ParteClicavel)")]
    public GameObject parteClicavel;

    [Header("PNG ampliado que aparece depois de coletar")]
    public Sprite spriteNovoAmpliado;

    [Header("Opcional: trocar também a imagem no cenário")]
    public Image imagemCenario;
    public Sprite spriteNovoCenario;

    bool aplicado;

    void OnEnable()
    {
        aplicado = false;
        Verificar();
    }

    void Update()
    {
        Verificar();
    }

    void Verificar()
    {
        if (aplicado) return;

        bool coletado = parteClicavel == null || !parteClicavel.activeSelf;
        if (!coletado) return;

        aplicado = true;

        if (ZoomManager.Instance != null && spriteNovoAmpliado != null)
            ZoomManager.Instance.imagemAmpliada.sprite = spriteNovoAmpliado;

        if (imagemCenario != null && spriteNovoCenario != null)
            imagemCenario.sprite = spriteNovoCenario;
    }
}