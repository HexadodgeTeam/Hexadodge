using UnityEngine;

public class BulletSpawner : MonoBehaviour, IBulletSpawner
{
    [Header("Parameters")]
    [SerializeField] private float bulletSpawnInterval = 0.3f;
    [SerializeField] private int maxBulletCount = 30;
    
    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;

    private Pool<Bullet> _pool;
    private float _bulletSpawnTimer = 0f;

    private void Awake()
    {
        _pool = new Pool<Bullet>(InstantiateBullet, DestroyBullet, maxBulletCount);
    }

    private void Update()
    {
        _bulletSpawnTimer += Time.deltaTime;
        
        if (_bulletSpawnTimer >= bulletSpawnInterval && !(_pool.Count > 0))
        {
            // TODO bulletData.spawning.Spawn(this, 1f);
            _bulletSpawnTimer -= bulletSpawnInterval;
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
