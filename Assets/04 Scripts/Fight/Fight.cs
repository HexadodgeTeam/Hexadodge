using UnityEngine;

public class Fight : AbstractFight
{
    [SerializeField] private FightData data;
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private Enemy enemy;

    private FightDataInstance _fightInstance;

    private void Start()
    {
        _fightInstance = data.Instance(enemy.TriggerAnim, bulletSpawner.SpawnBullet);
        enemy.Init(data.enemy);
        UIManager.Instance.SetLevelTitle(data.title);
    }

    private void Update()
    {
        _fightInstance.Tick(enemy.HealthRatio);
    }

    private void OnDrawGizmos()
    {
        data?.OnDrawGizmos();
    }

    public override void SetFight(FightData fightData)
    {
        data = fightData;
    }
}

public abstract class AbstractFight : MonoBehaviour
{
    public abstract void SetFight(FightData fightData);
}