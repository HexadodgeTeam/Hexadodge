using System.Collections.Generic;
using UnityEngine;

public class BulletTester : MonoBehaviour, IBulletSpawner
{
    public static bool ModifyOnPlay = false;
    
    [Header("Parameters")]
    [Tooltip("Scriptable à tester. Peut être BulletData.")]
    [SerializeField] private ScriptableObject testData;
    //[SerializeField] private float basePeriod = 5f;
    [SerializeField] private int maxBulletCount = 30;
    [SerializeField] private bool debugOneAtATime = false;
    
    [Header("BulletData")]
    [SerializeField] private float bulletSpawnInterval = 0.3f;
    [SerializeField] private Vector3 bulletSpawnPos;
    [SerializeField] private float bulletRotation;
    [SerializeField] private bool mirrorX;
    [SerializeField] private bool mirrorY;
    
    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;

    private Pool<Bullet> _pool;
    private float _bulletSpawnTimer = 0f;
    private BulletPatternInstance _bulletPatternInstance;
    private readonly List<(BulletData b, Vector2, float)> _spawns = new();

    private void Awake()
    {
        ModifyOnPlay = true;
        _pool = new Pool<Bullet>(InstantiateBullet, DestroyBullet, maxBulletCount);
        if (testData is BulletPattern bulletPattern)
            _bulletPatternInstance = bulletPattern.Instance();
    }

    private void Update()
    {
        if (testData is BulletData bulletData)
        {
            _bulletSpawnTimer += Time.deltaTime;
        
            if (_bulletSpawnTimer >= bulletSpawnInterval && !(debugOneAtATime && _pool.Count > 0))
            {
                SpawnBullet(bulletData, bulletSpawnPos, bulletRotation, (mirrorX, mirrorY));
                _bulletSpawnTimer -= bulletSpawnInterval;
            }
        }
        else if (testData is BulletPattern bulletPattern)
        {
            _bulletSpawnTimer += Time.deltaTime;
        
            _bulletPatternInstance.GetSpawns(_spawns, _bulletSpawnTimer / bulletSpawnInterval);
            foreach ((BulletData b, Vector2 p, float r) in _spawns)
            {
                //SpawnBullet(bulletData, p, r, );
            }
            if (_bulletSpawnTimer >= bulletSpawnInterval)
            {
                _bulletSpawnTimer -= bulletSpawnInterval;
            }
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

    public void SpawnBullet(BulletData bulletData, Vector3 position, float rotation, (bool, bool) mirror)
    {
        Bullet bullet = _pool.Spawn(position, rotation);
        if (bullet) bullet.Init(_pool, bulletData, mirror);
    }

    private void OnDrawGizmos()
    {
        if (testData is BulletData)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(bulletSpawnPos, 0.05f);
            Gizmos.color = Color.blue;
            Quaternion rotation = Quaternion.Euler(0, 0, bulletRotation);
            Vector3 direction = rotation * Vector3.right;
            Gizmos.DrawRay(bulletSpawnPos, direction * 0.5f);
        }
    }
}
