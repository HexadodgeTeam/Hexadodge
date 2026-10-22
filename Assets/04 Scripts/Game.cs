using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    
    public static float TimeScale { private get; set; }
    public static float ScaledTime { get; private set; }
    public static float DeltaTime => Time.deltaTime * TimeScale;

    [Header("Parameters")]
    [SerializeField] private GameData data;
    
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private FillManager fill;
    [SerializeField] private AbstractFight fight;
    
    private void Awake()
    {
        TimeScale = 1f;
        ScaledTime = 0f;
        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        if (FightDataTransfer.Instance) fight.SetFight(FightDataTransfer.Instance.fightData);
        fight.Init(data.fight);
    }

    private void FixedUpdate()
    {
        ScaledTime += DeltaTime;
    }

    public void SetColor(Color color, Gradient gradient)
    {
        player.UpdateColor(color);
        FillGrid.Instance.SetGradient(gradient);
        UIManager.Instance.fillUI.SetColor(color);
    }

    public bool TryAttack()
    {
        if (!fill.HasSkill) return false;
        
        int skillIndex = fill.GetSkillIndex();
        bool attacked = fight.TryAttack(skillIndex);
        
        if (!attacked) return false;
        
        fill.OnAttack();
        return true;
    }

    public void ChangeControlType(int value)
    {
        player.ChangeControlType(value);
    }

    public void SetPopUpActive(int value) => SetPopUp(value, true);
    public void SetPopUpInactive(int value) => SetPopUp(value, false);

    private static void SetPopUp(int value, bool active)
    {
        UIManager.Instance.SetPopUpActive(value, active);
    }

    public void EndFight()
    {
        fight.EndFight();
    }

    public void ResetTime()
    {
        TimeScale = 1f;
        ScaledTime = 0f;
        Time.timeScale = 1f;
    }
}
