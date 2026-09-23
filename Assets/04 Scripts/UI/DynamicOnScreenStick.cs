using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class DynamicOnScreenStick : OnScreenStick, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image stickBaseImg;
    [SerializeField] private Image stickKnobImg;
    [SerializeField] private Image stickZone;

    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = stickBaseImg.rectTransform.position;
        //stickBaseImg.enabled = false;
        //stickKnobImg.enabled = false;
    }

    public new void OnPointerDown(PointerEventData eventData)
    {
        stickBaseImg.rectTransform.position = eventData.position;
        stickZone.rectTransform.localPosition = Vector3.zero;
        //stickBaseImg.enabled = true;
        //stickKnobImg.enabled = true;
        base.OnPointerDown(eventData);
    }

    public new void OnPointerUp(PointerEventData eventData)
    {
        stickBaseImg.rectTransform.position = _startPosition;
        stickZone.rectTransform.localPosition = Vector3.zero;
        //stickBaseImg.enabled = false;
        //stickKnobImg.enabled = false;
        base.OnPointerUp(eventData);
    }
}