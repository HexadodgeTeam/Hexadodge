using UnityEngine;

public abstract class BulletSpawning : ScriptableObject
{
    public abstract void Spawn(IBulletSpawner spawner);
}