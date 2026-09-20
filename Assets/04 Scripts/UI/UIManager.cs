using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CountUI healthUI;
    [SerializeField] private SliderUI fillUI;
    [SerializeField] private Button attackButton;
    [SerializeField] private CountUI skillNumber;
    [SerializeField] private RectTransform[] thresholdIndicators;
    
    public static CountUI HealthUI;
    public static SliderUI FillUI;
    private static Button AttackButton;
    public static CountUI SkillNumber;
    private static RectTransform[] ThresholdIndicators;

    public static void SetAttackButton(bool ready)
    {
        AttackButton.interactable = ready;
    }

    public static void SetThresholdIndicators(float[] fractions)
    {
        for (var i = 0; i < Math.Min(fractions.Length, ThresholdIndicators.Length); i++)
        {
            RectTransform ti = ThresholdIndicators[i];
            ti.anchorMin = new Vector2(ti.anchorMin.x, fractions[i]);
            ti.anchorMax = new Vector2(ti.anchorMax.x, fractions[i]);
            ti.anchoredPosition = new Vector2(ti.anchoredPosition.x, 0f);
        }
    }
    
    private void Awake()
    {
        HealthUI = healthUI;
        FillUI = fillUI;
        AttackButton = attackButton;
        SkillNumber = skillNumber;
        ThresholdIndicators = thresholdIndicators;
    }
}
