using System.Collections;
using UnityEngine;

public class Fight : AbstractFight
{
    private static readonly int Death = Animator.StringToHash("SwitchPhase");
    
    [SerializeField] private FightData data;
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private Enemy enemy;

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
    public abstract void SetFight(FightData fightData);

    public abstract void EndFight();
}