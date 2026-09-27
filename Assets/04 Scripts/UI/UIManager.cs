using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CountUI healthUI;
    [SerializeField] private SliderUI fillUI;
    [SerializeField] private GameObject stick;
    [SerializeField] private SliderUI enemyHealthUI;
    [SerializeField] private RectTransform[] thresholdIndicators;
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private TMP_Dropdown controlTypeDropdown;

    public static CountUI HealthUI;
    public static SliderUI FillUI;
    public static SliderUI EnemyHealthUI;

    private static RectTransform[] _thresholdIndicators;
    private static GameOverPanel _gameOverPanel;

    private void Start()
    {
        RefreshControlType();
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

    public void SetControlType(int value)
    {
        PlayerPrefs.SetInt("ControlType", value);
        
        RefreshControlType();
    }

    private void Awake()
    {
        HealthUI = healthUI;
        FillUI = fillUI;
        _thresholdIndicators = thresholdIndicators;
        EnemyHealthUI = enemyHealthUI;
        _gameOverPanel = gameOverPanel;
    }

    private void RefreshControlType()
    {
        int controlType = PlayerPrefs.GetInt("ControlType");
        bool classic = controlType == (int)Control.Type.Classic;

        stick.SetActive(classic);
        playerInputHandler.ChangeControlType(controlType);
        controlTypeDropdown.value = controlType;
    }
}
