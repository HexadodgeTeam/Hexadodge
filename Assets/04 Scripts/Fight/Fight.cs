using System.Collections;
using UnityEngine;

public class Fight : AbstractFight
{
    private static readonly int Death = Animator.StringToHash("SwitchPhase");
    
    [Header("Parameters")]
    [SerializeField] private FightData data;

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
    }

    private void Update()
    {
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

        SetFight(data.nextFight);
    }

    public override void EndFight()
    {
        if (data.nextFight)
        {
            bulletSpawner.ClearBullets();
            enemy.TriggerAnim(Death);
            StartCoroutine(SetFightCoroutine());
        }
        else
        {
            UIManager.Instance.GameOver(true);
        }
    }

    private void OnDrawGizmos()
    {
        data?.OnDrawGizmos();
    }
}

public abstract class AbstractFight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected BulletSpawner bulletSpawner;
    [SerializeField] protected Enemy enemy;
    
    [Header("Attack")]
    [SerializeField] private int[] attackPowers;
    [SerializeField] private float attackDelay;
    [SerializeField] private Animator attackFx;
    
    [Header("Attack slowdown")]
    [SerializeField] private float attackSlowdownTimeScale = 0.1f;
    [SerializeField] private AnimationCurve attackSlowdownIn = new(new Keyframe(0, 0), new Keyframe(1, 1));
    [SerializeField] private float attackSlowdownInDuration = 0.2f;
    [SerializeField] private AnimationCurve attackSlowdownOut = new(new Keyframe(0, 0), new Keyframe(1, 1));
    [SerializeField] private float attackSlowdownOutDuration = 0.1f;
    
    public abstract void SetFight(FightData fightData);

    public abstract void EndFight();
    
    public bool TryAttack()
    {
        if (!HasSkill) return false;
            
        attackFx.SetTrigger(HitTriggers[CurrentSkillIndex]);
        grid.Empty();
        attackEvent.Invoke();
        StartCoroutine(AttackCoroutine(CurrentSkillIndex));
        StartCoroutine(AttackSlowdownCoroutine());
        CurrentSkillIndex = NoSkill;

        return true;
    }

    private IEnumerator AttackCoroutine(int skillIndex)
    {
        yield return new WaitForSeconds(enemyHitDelay);
        
        enemy.TakeDamage(attackPowers[skillIndex]);
    }

    private IEnumerator AttackSlowdownCoroutine()
    {
        float t = 0f;

        while (t < attackSlowdownDuration)
        {
            t += Time.deltaTime;

            Game.TimeScale = Mathf.Max(0f, attackSlowdownTimeScale.Evaluate(t / attackSlowdownDuration));

            yield return null;
        }

        Game.TimeScale = 1f;
    }
}