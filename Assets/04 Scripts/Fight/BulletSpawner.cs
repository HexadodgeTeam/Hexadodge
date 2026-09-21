using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private MoveBox moveBox;
    [SerializeField] private float bulletSpawnX;
    [SerializeField] private float minBulletSpawnY;
    [SerializeField] private float maxBulletSpawnY;
    [SerializeField] private float bulletSpawnInterval = 0.3f;
    [SerializeField] private Quaternion leftSpawnRotation = new Quaternion(0, 0, 1, 0);
    [SerializeField] private Quaternion rightSpawnRotation = new Quaternion(0, 0, 0, 1);

    private Pool<Bullet> _pool;
    private float _bulletSpawnTimer = 0f;

    private void Awake()
    {
        _pool = new Pool<Bullet>(bulletPrefab, 10);
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
        (Vector3 position, Quaternion rotation) spawn = GetRandomSpawnPosition();
        
        Instantiate(bulletPrefab, spawn.position, spawn.rotation);
    }

    private (Vector3, Quaternion) GetRandomSpawnPosition()
    {
        bool left = Random.value > 0.5f;
        
        float spawnY = Random.Range(minBulletSpawnY, maxBulletSpawnY);
        float spawnX = left ? -bulletSpawnX : bulletSpawnX;
        Quaternion spawnRotation = left ? leftSpawnRotation : rightSpawnRotation;

        return (new Vector3(spawnX, spawnY), spawnRotation);
    }
}
