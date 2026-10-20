using TMPro;
using UnityEngine;
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

    public CountUI healthUI;
    public SliderUI fillUI;
    public SliderUI enemyHealthUI;

    private GameOverPanel _gameOverPanel;

    private void Awake()
    {
        Instance = this;
        
        _gameOverPanel = gameOverPanel;
    }
    
    private void Start()
    {
        RefreshControlType();
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
}
