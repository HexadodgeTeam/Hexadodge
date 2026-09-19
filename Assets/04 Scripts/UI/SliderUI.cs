using UnityEngine;
using UnityEngine.UI;

public class SliderUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillArea;
    
    public void SetFraction(float fraction)
    {
        slider.normalizedValue = fraction;
        fillArea.enabled = fraction >= 0.01f;
    }

    public void SetColor(Color color)
    {
        fillArea.color = color;
    }
}