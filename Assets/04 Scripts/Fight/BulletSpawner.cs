using UnityEngine;

public class BulletSpawner : MonoBehaviour, IBulletSpawner
{
    [Header("Parameters")]
    [SerializeField] private BulletData bulletData;
    [SerializeField] private float bulletSpawnInterval = 0.3f;
    [SerializeField] private int maxBulletCount = 30;
    
    [Header("References")]
    [SerializeField] private MoveBox moveBox;
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
        
        if (_bulletSpawnTimer >= bulletSpawnInterval)
        {
            bulletData.spawning.Spawn(this);
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

    public void SpawnBullet(Vector3 position, float rotation)
    {
        Bullet bullet = _pool.Spawn(position, rotation);
        bullet?.Init(_pool, bulletData);
    }
}
