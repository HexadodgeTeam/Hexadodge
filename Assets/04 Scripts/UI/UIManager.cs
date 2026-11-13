using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static readonly int Hit = Animator.StringToHash("Hit");
    public static UIManager Instance { get; private set; }
    
    [Header("Parameters")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    [Header("References")]
    [SerializeField] private GameObject stick;
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private TMP_Dropdown controlTypeDropdown;
    [SerializeField] private Animator hitAnimator;
    [SerializeField] private BorderFill border;
    [SerializeField] private GameObject[] fillPopUps;
    [SerializeField] private TMP_Text levelTitle;
    [SerializeField] private UnityEvent attackEvent;
    [SerializeField] private Transform arenaMaskParent;
    [SerializeField] private ArenaBorder arenaMask;
    [SerializeField] private TMP_Text timerDisplay;

    public HealthUI healthUI;
    public SliderUI fillUI;
    public SliderUI enemyHealthUI;

    private GameOverPanel _gameOverPanel;
    private Color _currentColor;
    private float _timer = 0f;

    private void Awake()
    {
        Instance = this;
        
        _gameOverPanel = gameOverPanel;
    }
    
    private void Start()
    {
        RefreshControlType();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        int minutes = Mathf.FloorToInt(_timer / 60f);
        int seconds = Mathf.FloorToInt(_timer - minutes * 60);

        timerDisplay.text = $"{minutes:00}:{seconds:00}";
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
        Game.Instance.ChangeControlType(controlType);
        controlTypeDropdown.value = controlType;
    }

    public void SwitchToMainMenu()
    {
        Game.Instance.ResetTime();
        SceneManager.LoadScene(mainMenuScene);
    }
    
    public void SetThresholdIndicators(float[] fractions)
    {
        border.SetThresholdIndicators(fractions);
    }

    public void SetPopUpActive(int index, bool active)
    {
        fillPopUps[index].gameObject.SetActive(active);
    }

    public void SetLevelTitle(string title)
    {
        levelTitle.text = title;
    }

    public void OnAttack()
    {
        attackEvent.Invoke();
    }

    public void SetArenaMask(ArenaBorder borderPrefab)
    {
        if (arenaMask) Destroy(arenaMask.gameObject);
        arenaMask = Instantiate(borderPrefab, arenaMaskParent);
        fillUI = arenaMask.sliderUI;
        fillUI.SetColor(_currentColor);
    }

    public void SetColor(Color color)
    {
        _currentColor = color;
        fillUI.SetColor(color);
    }
}
