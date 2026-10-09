using UnityEngine;

public interface IBulletSpawner
{
    public void SpawnBullet(BulletData bulletData, Vector3 position, float rotation, (bool, bool) mirror);
}