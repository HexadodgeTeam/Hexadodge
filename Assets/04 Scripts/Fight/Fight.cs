using UnityEngine;

public class Fight : MonoBehaviour
{
    [SerializeField] private FightData data;
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private Enemy enemy;

    private FightDataInstance _fightInstance;

    private void Start()
    {
        _fightInstance = data.Instance(enemy.TriggerAnim, bulletSpawner.SpawnBullet);
        enemy.Init(data.enemy);
    }

    private void Update()
    {
        _fightInstance.Tick(enemy.HealthRatio);
    }

    private void OnDrawGizmos()
    {
        data?.OnDrawGizmos();
    }
}
