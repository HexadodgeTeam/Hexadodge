using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicStickFollow : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform stickBase;
    [SerializeField] private DynamicOnScreenStick dynamicOnScreenStick;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 basePos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, stickBase.position);
        Vector2 toTouch = eventData.position - basePos;
        float dist = toTouch.magnitude;
        if (dist > dynamicOnScreenStick.movementRange)
        {
            Vector2 dir = toTouch / dist;
            stickBase.position += (Vector3)(dir * (dist - dynamicOnScreenStick.movementRange));
        }
    }
}