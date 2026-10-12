using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Radial", menuName = "Scriptable Objects/Bullets/Spawning/Radial")]
public class BSRadial : BulletSpawning
{
    [Header("Position")]
    [Tooltip("Position du pôle (centre du cercle).")] [SerializeField]
    private Vector2Curve pole;
    
    [Tooltip("Distance par rapport au pôle (centre du cercle).")] [SerializeField]
    private FloatCurve distance = 1;

    [Tooltip("Angle par rapport à l'axe (1, 0).")] [SerializeField]
    private FloatCurve angle;

    [Header("Rotation")]
    [Tooltip("Est-ce que le bullet fait face au pôle à l'apparition ?")] [SerializeField]
    private bool facePole = true;

    [Tooltip("Offset de rotation.")] [SerializeField]
    private FloatCurve rotationOffset;
    
    protected override (Vector2 p, float r) GetSpawn(float time01, (bool x, bool y) mirror)
    {
        Vector2 center = pole.Eval(time01, Random.value, Random.value);
        float rad = angle.Eval(time01, Random.value) * Mathf.Deg2Rad;
        float dist = distance.Eval(time01, Random.value);
        Vector2 position = center + new Vector2(
            dist * Mathf.Cos(rad) * (mirror.x ? -1 : 1), 
            dist * Mathf.Sin(rad) * (mirror.y ? -1 : 1));

        Vector2 bulletToPlayer = center - position;
        float rotation = facePole ? Mathf.Atan2(bulletToPlayer.y, bulletToPlayer.x) * Mathf.Rad2Deg : 0
            + rotationOffset.Eval(time01, Random.value);

        return (position, rotation);
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
            
        Vector2 center = new Vector2(pole.x.Center, pole.y.Center);
        float radius = distance.Center;
        Gizmos.DrawWireSphere(center, radius);
                
        Gizmos.color = Color.blue;
        Quaternion rotation = Quaternion.Euler(0, 0, angle.Center);
        Vector3 direction = rotation * Vector3.right;
        Gizmos.DrawRay(center, direction * radius);
    }
}