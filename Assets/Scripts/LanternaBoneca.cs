using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LanternaBoneca : MonoBehaviour, IPointerClickHandler
{
    public CasaDeBonecas casa;
    public Lanterna tipo;

    void Awake()
    {
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (casa != null) casa.AlternarLanterna(tipo);
    }
}