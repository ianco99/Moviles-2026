using UnityEngine;
using UnityEngine.EventSystems;

public class SteeringWheelImage : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 screenPos = eventData.position;
        Debug.Log(screenPos);
    }
}
