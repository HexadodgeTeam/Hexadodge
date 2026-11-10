using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    
    public static float TimeScale { private get; set; }
    public static float ScaledTime { get; private set; }
    public static float DeltaTime => Time.deltaTime * TimeScale;
    public bool IsInFight { get; private set; }

    [Header("Parameters")]
    [SerializeField] private GameData data;
    
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private FillManager fill;
    [SerializeField] private AbstractFight fight;
    [SerializeField] private GameObject background;
    
    private void Awake()
    {
        TimeScale = 1f;
        ScaledTime = 0f;
        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        if (FightDataTransfer.Instance) SetFight(FightDataTransfer.Instance.fightData);
        fight.Init(data.fight);
        fill.Init(data.baseSkills);
        StartFight();
    }

    private void FixedUpdate()
    {
        ScaledTime += DeltaTime;
    }

    public void SetColor(Color color, Gradient gradient)
    {
        player.UpdateColor(color);
        fight.SetAttackColor(color);
        FillGrid.Instance.SetGradient(gradient);
        UIManager.Instance.fillUI.SetColor(color);
    }

    public bool TryAttack()
    {
        if (!fill.HasSkill) return false;
        
        int skillIndex = fill.GetSkillIndex();
        bool attacked = fight.TryAttack(skillIndex);
        
        if (!attacked) return false;
        
        foreach (SkillData skill in data.baseSkills)
        {
            skill.OnAttack(skillIndex);
        }
        fill.OnAttack();
        
        return true;
    }

    public void SetFight(FightData fightData)
    {
        if (fightData.backgroundPrefab)
        {
            Destroy(background);
            background = Instantiate(fightData.backgroundPrefab, transform);
        }
        
        if (fightData.gridPrefab) FillGrid.Instance.SetGrid(fightData.gridPrefab);
        player.InitPaintAction();
        fight.SetFight(fightData);
        
        // TP player to spawn position.
        player.Teleport(fightData.playerSpawnPoint);
        
        StartFight();
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

    private void StartFight()
    {
        IsInFight = true;
        FillGrid.Instance.Active = true;
    }
    
    public void EndFight()
    {
        IsInFight = false;
        fight.EndFight();
    }

    public void SetTransfer()
    {
        fight.SetTransfer();
    }

    public void ResetTime()
    {
        TimeScale = 1f;
        ScaledTime = 0f;
        Time.timeScale = 1f;
    }
}
