using UnityEngine;

public class BulletSpawner : MonoBehaviour, IBulletSpawner
{
    [Header("Parameters")]
    [SerializeField] private BulletData bulletData;
    [SerializeField] private float bulletSpawnInterval = 0.3f;
    
    [Header("References")]
    [SerializeField] private MoveBox moveBox;
    [SerializeField] private Bullet bulletPrefab;

    private Pool<Bullet> _pool;
    private float _bulletSpawnTimer = 0f;

    private void Awake()
    {
        _pool = new Pool<Bullet>(InstantiateBullet, DestroyBullet, 10);
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
        Bullet bullet = Instantiate(bulletPrefab, transform);
        bullet.Init(_pool, bulletData.movement, bulletData.properties);
        return bullet;
    }

    private static void DestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    public void SpawnBullet(Vector3 position, float rotation)
    {
        _pool.Spawn(position, rotation);
    }
}
