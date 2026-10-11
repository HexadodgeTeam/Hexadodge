using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Radial", menuName = "Scriptable Objects/Bullets/Spawning/Radial")]
public class BSRadial : BulletSpawning
{
    [Tooltip("Position du pôle (centre du cercle).")] [SerializeField]
    private Vector2Curve pole;
    
    [Tooltip("Distance par rapport au pôle (centre du cercle).")] [SerializeField]
    private FloatCurve distance;

    [Tooltip("Angle par rapport à l'axe (1, 0).")] [SerializeField]
    private FloatCurve angle;

    [Tooltip("Est-ce que le bullet fait face au joueur à l'apparition ?")]
    private bool facePlayer = true;

    [Tooltip("Offset de rotation.")] [SerializeField]
    private FloatCurve rotationOffset;
    
    protected override (Vector2 p, float r) GetSpawn(float time01, (bool x, bool y) mirror)
    {
        float rad = angle.Eval(time01, Random.value) * Mathf.Deg2Rad;
        float dist = distance.Eval(time01, Random.value);
        Vector2 position = pole.Eval(time01, Random.value, Random.value)
                           + new Vector2(
                               dist * Mathf.Cos(rad) * (mirror.x ? -1 : 1), 
                               dist * Mathf.Sin(rad) * (mirror.y ? -1 : 1));

        Vector2 bulletToPlayer = Player.Pos - position;
        float rotation = facePlayer ? Mathf.Atan2(bulletToPlayer.y, bulletToPlayer.x) * Mathf.Rad2Deg : 0
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
        /* TODO (float, float) angleRange = rotations[i].GetAngleRange();
        Quaternion rotation = Quaternion.Euler(0, 0, angleRange.Item1);
        Quaternion rotation2 = Quaternion.Euler(0, 0, angleRange.Item2);
        Vector3 direction = rotation * Vector3.right;
        Vector3 direction2 = rotation2 * Vector3.right;
        Gizmos.DrawLine(center, center + direction * 0.5f);
        Gizmos.DrawLine(center, center + direction2 * 0.5f);*/
    }
}