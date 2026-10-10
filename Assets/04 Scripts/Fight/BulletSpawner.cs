using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour, IBulletSpawner
{
    [Header("Parameters")]
    [SerializeField] private BulletPattern bulletPattern;
    [SerializeField] private float bulletSpawnInterval = 0.3f;
    [SerializeField] private int maxBulletCount = 30;
    [SerializeField] private float basePeriod = 5f;
    
    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;

    private Pool<Bullet> _pool;
    private BulletPatternInstance _bulletPatternInstance;
    private float _bulletSpawnTimer = 0f;
    private readonly List<(BulletData, Vector2, float, (bool, bool))> _spawns = new();

    private void Awake()
    {
        _pool = new Pool<Bullet>(InstantiateBullet, DestroyBullet, maxBulletCount);
        _bulletPatternInstance = bulletPattern.Instance();
    }

    private void Update()
    {
        _bulletSpawnTimer += Time.deltaTime;
        
        _bulletPatternInstance.GetSpawns(_spawns, _bulletSpawnTimer / basePeriod);
        foreach ((BulletData b, Vector2 p, float r, (bool, bool) mirror) in _spawns)
        {
            SpawnBullet(b, p, r, mirror);
        }
        _spawns.Clear();
            
        if (_bulletSpawnTimer >= basePeriod)
        {
            _bulletSpawnTimer = 0f;
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
}
