using UnityEngine;

public abstract class BulletSpawning : ScriptableObject
{
    [Tooltip("Un sous-spawning qui s'assemblera avec celui-ci. Les positions et rotations sont additionnées.")]
    [SerializeField] internal BulletSpawning subSpawning;

    public (Vector2 p, float r) GetCombinedSpawn(float time01, (bool x, bool y) mirror, float spawnTime01)
    {
        (Vector2 p, float r) subSpawn = subSpawning
            ? subSpawning.GetCombinedSpawn(time01, mirror, spawnTime01)
            : (Vector2.zero, 0f);
        
        (Vector2 p, float r) spawn = GetSpawn(time01, mirror, spawnTime01);
        
        return (subSpawn.p + spawn.p, subSpawn.r + spawn.r);
    }
    
    protected abstract (Vector2 p, float r) GetSpawn(float time01, (bool x, bool y) mirror, float count01);
    
    public abstract void OnDrawGizmos();
}