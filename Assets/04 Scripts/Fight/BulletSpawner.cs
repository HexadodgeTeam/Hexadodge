using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int maxBulletCount = 30;
    
    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;

    private Pool<Bullet> _pool;
    private FightDataInstance _fightData;

    private void Awake()
    {
        _pool = new Pool<Bullet>(InstantiateBullet, DestroyBullet, maxBulletCount);
    }

    private Bullet InstantiateBullet()
    {
        return Instantiate(bulletPrefab, transform);
    }

    private static void DestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    public void SpawnBullet(BulletSpawn data)
    {
        Bullet bullet = _pool.Spawn(data.position, data.rotation);
        if (bullet) bullet.Init(_pool, data.bullet, data.mirror);
    }
}

public readonly struct BulletSpawn
{
    public readonly BulletData bullet;
    public readonly Vector2 position;
    public readonly float rotation;
    public readonly (bool, bool) mirror;

    public BulletSpawn(BulletData bullet, Vector2 position, float rotation, (bool, bool) mirror)
    {
        this.bullet = bullet;
        this.position = position;
        this.rotation = rotation;
        this.mirror = mirror;
    }
}