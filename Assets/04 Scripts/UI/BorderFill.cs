using System;
using UnityEngine;

public class BorderFill : MonoBehaviour
{
    [SerializeField] private RectTransform[] thresholdIndicators;
    
    private RectTransform[] _thresholdIndicators;

    private void Awake()
    {
        _thresholdIndicators = thresholdIndicators;
    }

    public void SetThresholdIndicators(float[] fractions)
    {
        for (var i = 0; i < Math.Min(fractions.Length, _thresholdIndicators.Length); i++)
        {
            RectTransform ti = _thresholdIndicators[i];
            ti.anchorMin = new Vector2(ti.anchorMin.x, fractions[i]);
            ti.anchorMax = new Vector2(ti.anchorMax.x, fractions[i]);
            ti.anchoredPosition = new Vector2(ti.anchoredPosition.x, 0f);
        }
    }
}
