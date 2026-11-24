using UnityEngine;
using UnityEngine.EventSystems;

public class RedirecionarDrop : MonoBehaviour, IDropHandler
{
    public GameObject alvo;

    public void OnDrop(PointerEventData e)
    {
        if (alvo != null)
            ExecuteEvents.Execute(alvo, e, ExecuteEvents.dropHandler);
    }
}