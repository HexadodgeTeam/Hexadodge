using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static readonly int Hit = Animator.StringToHash("Hit");
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private GameObject stick;
    [SerializeField] private RectTransform[] thresholdIndicators;
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private TMP_Dropdown controlTypeDropdown;
    [SerializeField] private Animator hitAnimator;
    [SerializeField] private string mainMenuScene = "MainMenu";

    public CountUI healthUI;
    public SliderUI fillUI;
    public SliderUI enemyHealthUI;

    private RectTransform[] _thresholdIndicators;
    private GameOverPanel _gameOverPanel;

    private void Awake()
    {
        Instance = this;
        
        _thresholdIndicators = thresholdIndicators;
        _gameOverPanel = gameOverPanel;
    }
    
    private void Start()
    {
        RefreshControlType();
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

    public void GameOver(bool win)
    {
        _gameOverPanel.Display(win);
    }

    public void SetControlType(int value)
    {
        PlayerPrefs.SetInt("ControlType", value);
        
        RefreshControlType();
    }
    
    public void OnHit() => hitAnimator.SetTrigger(Hit);

    private void RefreshControlType()
    {
        int controlType = PlayerPrefs.GetInt("ControlType");
        bool classic = controlType == (int)Control.Type.Classic;

        stick.SetActive(classic);
        playerInputHandler.ChangeControlType(controlType);
        controlTypeDropdown.value = controlType;
    }

    public void SwitchToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
