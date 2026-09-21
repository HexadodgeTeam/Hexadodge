using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CountUI healthUI;
    [SerializeField] private SliderUI fillUI;
    [SerializeField] private Button attackButton;
    [SerializeField] private CountUI skillNumber;
    [SerializeField] private SliderUI enemyHealthUI;
    [SerializeField] private RectTransform[] thresholdIndicators;
    [SerializeField] private GameOverPanel gameOverPanel;
    
    public static CountUI HealthUI;
    public static SliderUI FillUI;
    public static CountUI SkillNumber;
    public static SliderUI EnemyHealthUI;
    
    private static Button _attackButton;
    private static RectTransform[] _thresholdIndicators;
    private static GameOverPanel _gameOverPanel;

    public static void SetAttackButton(bool ready)
    {
        _attackButton.interactable = ready;
    }

    public static void SetThresholdIndicators(float[] fractions)
    {
        for (var i = 0; i < Math.Min(fractions.Length, _thresholdIndicators.Length); i++)
        {
            RectTransform ti = _thresholdIndicators[i];
            ti.anchorMin = new Vector2(ti.anchorMin.x, fractions[i]);
            ti.anchorMax = new Vector2(ti.anchorMax.x, fractions[i]);
            ti.anchoredPosition = new Vector2(ti.anchoredPosition.x, 0f);
        }
    }

    public static void GameOver(bool win)
    {
        _gameOverPanel.Display(win);
    }
    
    private void Awake()
    {
        HealthUI = healthUI;
        FillUI = fillUI;
        _attackButton = attackButton;
        SkillNumber = skillNumber;
        _thresholdIndicators = thresholdIndicators;
        EnemyHealthUI = enemyHealthUI;
        _gameOverPanel = gameOverPanel;
    }
}
