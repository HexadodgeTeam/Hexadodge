using UnityEngine;

public abstract class BulletSpawning : ScriptableObject
{
    public abstract (Vector2 p, float r)? GetSpawn(float time01, (bool x, bool y) mirror);
    
    public abstract void OnDrawGizmos();
}