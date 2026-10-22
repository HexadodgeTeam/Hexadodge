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
        Bullet bullet = _pool.Spawn(data.Position, data.Rotation);
        if (bullet) bullet.Init(_pool, data.Bullet, data.Mirror);
    }

    public void ClearBullets()
    {
        _pool.Clear();
    }
    
    public bool HasBulletSpawned()
    {
        return _pool.Count > 0;
    }
}

public readonly struct BulletSpawn
{
    public readonly BulletData Bullet;
    public readonly Vector2 Position;
    public readonly float Rotation;
    public readonly (bool, bool) Mirror;

    public BulletSpawn(BulletData bullet, Vector2 position, float rotation, (bool, bool) mirror)
    {
        Bullet = bullet;
        Position = position;
        Rotation = rotation;
        Mirror = mirror;
    }
}