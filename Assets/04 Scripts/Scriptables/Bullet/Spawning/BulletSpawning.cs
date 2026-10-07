using UnityEngine;

public abstract class BulletSpawning : ScriptableObject
{
    public abstract void Spawn(IBulletSpawner spawner, float time);
    
    public abstract void OnDrawGizmos();
}