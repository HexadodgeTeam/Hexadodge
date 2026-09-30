using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "Scriptable Objects/Bullets/Spawning/Test")]
public class BSTest : BulletSpawning
{
    [SerializeField] private float bulletSpawnX = 2.5f;
    [SerializeField] private float minBulletSpawnY = -2.2f;
    [SerializeField] private float maxBulletSpawnY = 2.2f;
    [SerializeField] private float leftSpawnRotation = 0f;
    [SerializeField] private float rightSpawnRotation = 180f;

    public override void Spawn(IBulletSpawner spawner)
    {
        bool left = Random.value > 0.5f;
        
        float x = left ? -bulletSpawnX : bulletSpawnX;
        float y = Random.Range(minBulletSpawnY, maxBulletSpawnY);
        float rotation = left ? leftSpawnRotation : rightSpawnRotation;
        
        spawner.SpawnBullet(new Vector3(x, y), rotation);
    }
}