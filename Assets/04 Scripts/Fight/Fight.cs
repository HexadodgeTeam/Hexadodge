using System.Collections;
using UnityEngine;

public class Fight : AbstractFight
{
    private static readonly int Death = Animator.StringToHash("SwitchPhase");
    
    [Header("Parameters")]
    [SerializeField] public FightData data;
    [SerializeField] private GameObject arena;
    public SubmitRunFonction subRun;
    
    private FightDataInstance _fightInstance;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _fightInstance = data.Instance(enemy.TriggerAnim, bulletSpawner.SpawnBullet);
        enemy.Init(data.enemy);
        UIManager.Instance.SetLevelTitle(data.title);
        if (data.arenaPrefab)
        {
            Destroy(arena);
            arena = Instantiate(data.arenaPrefab, transform);
        }
        if (data.borderPrefab) UIManager.Instance.SetArenaMask(data.borderPrefab);
    }

    private void Update()
    {
        if (!Game.Instance.IsInFight) return;
        
        _fightInstance.Tick(enemy.HealthRatio);
    }

    public override void SetFight(FightData fightData)
    {
        data = fightData;
        Init();
    }

    private IEnumerator SetFightCoroutine()
    {
        yield return new WaitForSeconds(data.enemy.transitionDuration);

        Game.Instance.SetFight(data.nextFight);
    }

    public override void WinFight()
    {
        _fightInstance.End();
        subRun.subRun();
        EarnRewards();
        
        if (data.nextFight)
        {
            
            LevelLeaderboard.OnLevelStart(data.nextFight.title);
            enemy.TriggerAnim(Death);
            StartCoroutine(SetFightCoroutine());
        }
        else
        {
            UIManager.Instance.GameOver(true);
        }
    }

    private void EarnRewards()
    {
        foreach (SkillData skill in data.skillRewards)
        {
            PlayerState.Instance.UpcomingSkills.Add(skill);
        }

        foreach (FightData fight in data.fightUnlocks)
        {
            PlayerState.Instance.Fights[fight].unlocked = true;
        }
        
        PlayerState.Instance.SaveData();
    }

    public override void SetTransfer()
    {
        FightDataTransfer.Instance.SetData(data);
    }

    private void OnDrawGizmos()
    {
        data?.OnDrawGizmos();
    }
}