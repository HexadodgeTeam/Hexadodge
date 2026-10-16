using UnityEngine;

public class Fight : MonoBehaviour
{
    [SerializeField] private FightData data;
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private Enemy enemy;
    [SerializeField] private bool instanceFight = false;

    private FightDataInstance _fightInstance;

    private void Start()
    {
        if (instanceFight) _fightInstance = data.Instance(enemy.TriggerAnim, bulletSpawner.SpawnBullet);
        enemy.Init(data.enemy);
    }

    private void Update()
    {
        if (instanceFight) _fightInstance.Tick(enemy.HealthRatio);
    }

    private void OnDrawGizmos()
    {
        data?.OnDrawGizmos();
    }
}
