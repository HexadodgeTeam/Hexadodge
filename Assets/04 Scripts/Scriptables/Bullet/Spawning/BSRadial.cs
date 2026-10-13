using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Radial", menuName = "Scriptable Objects/Bullets/Spawning/Radial")]
public class BSRadial : BulletSpawning
{
    [Header("Position")]
    [Tooltip("Position du pôle (centre du cercle).")]
    [SerializeField]
    private Vector2Curve pole;
    
    [Tooltip("Évaluer le X du pôle depuis le nombre de bullets instanciés au lieu de la période.")]
    [SerializeField]
    private bool poleXFromCount = false;
    
    [Tooltip("Évaluer le Y du pôle depuis le nombre de bullets instanciés au lieu de la période.")]
    [SerializeField]
    private bool poleYFromCount = false;
    
    [Tooltip("Distance par rapport au pôle (centre du cercle).")]
    [SerializeField]
    private FloatCurve distance = 1;
    
    [Tooltip("Évaluer la distance depuis le nombre de bullets instanciés au lieu de la période.")]
    [SerializeField]
    private bool distanceFromCount = false;

    [Tooltip("Angle par rapport à l'axe (1, 0).")]
    [SerializeField]
    private FloatCurve angle;
    
    [Tooltip("Évaluer l'angle depuis le nombre de bullets instanciés au lieu de la période.")]
    [SerializeField]
    private bool angleFromCount = false;

    [Header("Rotation")]
    [Tooltip("Est-ce que le bullet fait face au pôle à l'apparition ?")]
    [SerializeField]
    private bool facePole = true;

    [Tooltip("Offset de rotation du bullet.")]
    [SerializeField]
    private FloatCurve rotationOffset;
    
    [Tooltip("Évaluer l'offset de rotation du bullet depuis le nombre de bullets instanciés au lieu de la période.")]
    [SerializeField]
    private bool rotationOffsetFromCount = false;
    
    protected override (Vector2 p, float r) GetSpawn(float time01, (bool x, bool y) mirror, float count01)
    {
        Vector2 center = new Vector2(
            pole.x.Eval(poleXFromCount ? count01 : time01, Random.value),
            pole.y.Eval(poleYFromCount ? count01 : time01, Random.value));
        float rad = angle.Eval(angleFromCount ? count01 : time01, Random.value) * Mathf.Deg2Rad;
        float dist = distance.Eval(distanceFromCount ? count01 : time01, Random.value);
        Vector2 position = center + new Vector2(
            dist * Mathf.Cos(rad) * (mirror.x ? -1 : 1), 
            dist * Mathf.Sin(rad) * (mirror.y ? -1 : 1));

        Vector2 bulletToPlayer = center - position;
        float rotation = facePole ? Mathf.Atan2(bulletToPlayer.y, bulletToPlayer.x) * Mathf.Rad2Deg : 0
            + rotationOffset.Eval(rotationOffsetFromCount ? count01 : time01, Random.value);

        return (position, rotation);
    }

    public override void OnDrawGizmos()
    {
        if (pole.x.CanGizmo && pole.y.CanGizmo && distance.CanGizmo)
        {
            Gizmos.color = Color.red;
            Vector2 center = new Vector2(pole.x.Center, pole.y.Center);
            float radius = distance.Center;
            Gizmos.DrawWireSphere(center, radius);
            
            if (angle.CanGizmo)
            {
                Gizmos.color = Color.blue;
                Quaternion rotation = Quaternion.Euler(0, 0, angle.Center);
                Vector3 direction = rotation * Vector3.right;
                Gizmos.DrawRay(center, direction * radius);
            }
        }
    }
}