using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletTester : MonoBehaviour
{
    public static bool ModifyOnPlay = false;
    
    [Header("Parameters")]
    [Tooltip("Scriptable à tester. Peut être BulletData, BulletPattern, PatternSequence, FightPhase ou FightData.")]
    [SerializeField] private ScriptableObject testData;
    [SerializeField] private float basePeriod = 5f;
    [SerializeField] private int maxBulletCount = 30;
    [SerializeField] private bool debugOneAtATime = false;
    [SerializeField] private InputAction reset;

    [Header("If not FightData")]
    [SerializeField] private EnemyData enemyData;

    [Header("If not FightData or FightPhase")]
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Gradient gradient;
    
    [Header("If BulletData")]
    [SerializeField] private float bulletSpawnInterval = 0.3f;
    [SerializeField] private Vector3 bulletSpawnPos;
    [SerializeField] private float bulletRotation;
    [SerializeField] private bool mirrorX;
    [SerializeField] private bool mirrorY;
    
    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Enemy enemy;

    private Pool<Bullet> _pool;
    private float _bulletSpawnTimer = 0f;
    private BulletPatternInstance _bulletPatternInstance;
    private PatternSequenceInstance _patternSequenceInstance;
    private FightPhaseInstance _fightPhaseInstance;
    private FightDataInstance _fightInstance;

    private void Awake()
    {
        ModifyOnPlay = true;
        _pool = new Pool<Bullet>(InstantiateBullet, DestroyBullet, maxBulletCount);
    }

    private void Start()
    {
        Refresh();
        reset.Enable();
        
        if (testData is FightData fightData) enemy.Init(fightData.enemy);
        else enemy.Init(enemyData);

        if (testData is not FightData && testData is not FightPhase) Game.Instance.SetColor(color, gradient);
    }

    private void Refresh()
    {
        switch (testData)
        {
            case BulletPattern bulletPattern:
                _bulletPatternInstance = bulletPattern.Instance(SpawnBullet);
                break;
            case PatternSequence patternSequence:
                _patternSequenceInstance = patternSequence.Instance(enemy.TriggerAnim, SpawnBullet);
                _patternSequenceInstance.Start();
                break;
            case FightPhase fightPhase:
                _fightPhaseInstance = fightPhase.Instance(enemy.TriggerAnim, SpawnBullet);
                _fightPhaseInstance.Start();
                break;
            case FightData fightData:
                _fightInstance = fightData.Instance(enemy.TriggerAnim, SpawnBullet);
                break;
        }
    }

    private void Update()
    {
        if (reset.WasPressedThisFrame())
        {
            Refresh();
        }

        switch (testData)
        {
            case BulletData bulletData:
                _bulletSpawnTimer += Game.DeltaTime;
        
                if (_bulletSpawnTimer >= bulletSpawnInterval && !(debugOneAtATime && _pool.Count > 0))
                {
                    SpawnBullet(new BulletSpawn(bulletData, bulletSpawnPos, bulletRotation, (mirrorX, mirrorY)));
                    _bulletSpawnTimer -= bulletSpawnInterval;
                }

                break;
            case BulletPattern:
                _bulletSpawnTimer += Game.DeltaTime;
        
                _bulletPatternInstance.Tick(_bulletSpawnTimer / basePeriod);
            
                if (_bulletSpawnTimer >= basePeriod)
                {
                    _bulletSpawnTimer = 0f;
                }

                break;
            case PatternSequence:
                _bulletSpawnTimer += Game.DeltaTime;
                
                _patternSequenceInstance.Tick(_bulletSpawnTimer / basePeriod);
                
                if (_patternSequenceInstance.Ended(_bulletSpawnTimer / basePeriod))
                    _patternSequenceInstance.ReRoll();
                
                if (_bulletSpawnTimer >= basePeriod)
                {
                    _bulletSpawnTimer = 0f;
                }
                
                break;
            case FightPhase:
                _fightPhaseInstance.Tick();
                break;
            case FightData:
                _fightInstance.Tick(enemy.HealthRatio);
                break;
        }
    }

    private Bullet InstantiateBullet()
    {
        return Instantiate(bulletPrefab, transform);
    }

    private static void DestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    private void SpawnBullet(BulletSpawn spawn)
    {
        Bullet bullet = _pool.Spawn(spawn.position, spawn.rotation);
        if (bullet) bullet.Init(_pool, spawn.bullet, spawn.mirror);
    }

    private void OnDrawGizmos()
    {
        switch (testData)
        {
            case BulletData:
                // Draw spawn point and rotation gizmo.
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(bulletSpawnPos, 0.05f);
                Gizmos.color = Color.blue;
                Quaternion rotation = Quaternion.Euler(0, 0, bulletRotation);
                Vector3 direction = rotation * Vector3.right;
                Gizmos.DrawRay(bulletSpawnPos, direction * 0.5f);
                break;
            case IGizmosDrawer gizmosDrawer:
                gizmosDrawer.OnDrawGizmos();
                break;
        }
    }
}
