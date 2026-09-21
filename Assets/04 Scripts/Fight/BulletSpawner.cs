using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private MoveBox moveBox;
    [SerializeField] private float bulletSpawnX;
    [SerializeField] private float minBulletSpawnY;
    [SerializeField] private float maxBulletSpawnY;
    [SerializeField] private float bulletSpawnInterval = 0.3f;
    [SerializeField] private float leftSpawnRotation = 0f;
    [SerializeField] private float rightSpawnRotation = 180f;

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
            SpawnBullet();
            _bulletSpawnTimer -= bulletSpawnInterval;
        }
    }

    private void SpawnBullet()
    {
        (Vector3 position, float rotation) spawn = GetRandomSpawnPosition();
        
        _pool.Spawn(spawn.position, spawn.rotation);
    }

    private Bullet InstantiateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform);
        bullet.Init(_pool);
        return bullet;
    }

    private void DestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    private (Vector3, float) GetRandomSpawnPosition()
    {
        bool left = Random.value > 0.5f;
        
        float spawnY = Random.Range(minBulletSpawnY, maxBulletSpawnY);
        float spawnX = left ? -bulletSpawnX : bulletSpawnX;
        float spawnRotation = left ? leftSpawnRotation : rightSpawnRotation;

        return (new Vector3(spawnX, spawnY), spawnRotation);
    }
}
