using UnityEngine;

[CreateAssetMenu(fileName = "Position", menuName = "Scriptable Objects/Bullets/Spawning/Position")]
public class BSPosition : BulletSpawning
{
    [Tooltip("Ensemble de positions où le bullet peut apparaître au hasard.")]
    [SerializeField] private Vector2[] positions;
    
    [Tooltip("Ensemble de rotations dans lesquelles le bullet peut se diriger au hasard à l'apparition.\n" +
             "Laisser vide pour une rotation totalement aléatoire dans les 360 degrés.")]
    [SerializeField] private float[] rotations;

    public override void Spawn(IBulletSpawner spawner)
    {
        Vector3 position = positions[Random.Range(0, positions.Length)];
        float rotation = rotations.Length > 0 ? rotations[Random.Range(0, rotations.Length)] : Random.Range(0f, 360f);
        
        spawner.SpawnBullet(position, rotation);
    }
}