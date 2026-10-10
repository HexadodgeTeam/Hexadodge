using UnityEngine;

[CreateAssetMenu(fileName = "Position", menuName = "Scriptable Objects/Bullets/Spawning/Position")]
public class BSPosition : BulletSpawning
{
    [Tooltip("Ensemble de positions où le bullet peut apparaître au hasard.")]
    [SerializeField] private Vector2Curve[] positions;
    
    [Tooltip("Ensemble de rotations dans lesquelles le bullet peut se diriger au hasard à l'apparition.\n" +
             "Laisser vide pour une rotation totalement aléatoire dans les 360 degrés.")]
    [SerializeField] private FloatCurve[] rotations;
    
    [Tooltip("Appairer les positions et les rotations des 2 tableaux ci-dessus, par index.\n" +
             "Exemple : la première position définie est associée à la première rotation définie.\n" +
             "Si activé, les deux tableaux doivent avoir le même nombre d'éléments.")]
    [SerializeField] private bool pairPositionWithRotation;

    public override (Vector2 p, float r)? GetSpawn(float time, (bool x, bool y) mirror)
    {
        if (pairPositionWithRotation && positions.Length != rotations.Length)
        {
            Debug.LogWarning("Le nombre de positions diffère du nombre de rotations. Apparition ignorée.");
            return null;
        }
        
        int randomPosIndex = Random.Range(0, positions.Length);
        int randomRotIndex = pairPositionWithRotation ? randomPosIndex : Random.Range(0, rotations.Length);

        Vector3 position = positions[randomPosIndex].Eval(time, Random.value, Random.value);
        float rotation = rotations.Length > 0
            ? rotations[randomRotIndex].Eval(time, Random.value)
            : Random.Range(0f, 360f);

        if (mirror.x) rotation = 180 - rotation;
        if (mirror.y) rotation = 360 - rotation;

        return (position, rotation);
    }

    public override void OnDrawGizmos()
    {
        for (var i = 0; i < positions.Length; i++)
        {
            Gizmos.color = Color.red;
            Rect? rect = positions[i].GetGizmosRect();
            
            if (rect.HasValue)
            {
                Vector3 center = rect.Value.center;
                Gizmos.DrawWireCube(center, rect.Value.size);
                
                if (pairPositionWithRotation && rotations[i].CanGizmo)
                {
                    Gizmos.color = Color.blue;
                    (float, float) angleRange = rotations[i].GetAngleRange();
                    Quaternion rotation = Quaternion.Euler(0, 0, angleRange.Item1);
                    Quaternion rotation2 = Quaternion.Euler(0, 0, angleRange.Item2);
                    Vector3 direction = rotation * Vector3.right;
                    Vector3 direction2 = rotation2 * Vector3.right;
                    Gizmos.DrawLine(center, center + direction * 0.5f);
                    Gizmos.DrawLine(center, center + direction2 * 0.5f);
                }
            }
        }
    }
}