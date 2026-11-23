using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomTrigger : MonoBehaviour, IPointerClickHandler
{
    public enum Tipo { Imagem, Terminal }

    public Tipo tipo = Tipo.Imagem;
    public Sprite spriteAmpliado; // usado só quando Tipo = Imagem

    [Header("Opcional: objeto que só aparece neste zoom (ex.: parte clicável que dá item)")]
    public GameObject conteudoExtra;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ZoomManager.Instance == null || ZoomManager.EstaAberto) return;

        if (tipo == Tipo.Terminal) ZoomManager.Instance.AbrirTerminal();
        else ZoomManager.Instance.AbrirImagem(spriteAmpliado, conteudoExtra);
    }
}