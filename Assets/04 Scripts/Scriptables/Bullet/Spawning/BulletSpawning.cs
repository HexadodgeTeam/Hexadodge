using UnityEngine;

public abstract class BulletSpawning : PeriodData
{
    public abstract (Vector2 p, float r)? GetSpawn(float time, (bool x, bool y) mirror);
    
    public abstract void OnDrawGizmos();
}