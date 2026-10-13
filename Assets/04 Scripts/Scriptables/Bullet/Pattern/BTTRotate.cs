using UnityEngine;

[CreateAssetMenu(fileName = "Rotate", menuName = "Scriptable Objects/Bullets/ThrowTransform/Rotate")]
public class TTRotate : ThrowTransform
{
    [Tooltip("Centre de la rotation.")]
    [SerializeField] internal Vector2Curve center;

    [Tooltip("Angle de rotation.")]
    [SerializeField] internal FloatCurve angle;

    public override (Vector2 p, float r) TransformSpawn(float time01, float random01, (Vector2 p, float r) spawn)
    {
        Vector2 c = center.Eval(time01, random01, random01);
        float a = angle.Eval(time01, random01);
        float rad = Mathf.Deg2Rad * a;
        spawn.p -= c;

        spawn.p = new Vector2(
            spawn.p.x * Mathf.Cos(rad) - spawn.p.y * Mathf.Sin(rad),
            spawn.p.x * Mathf.Sin(rad) + spawn.p.y * Mathf.Cos(rad));

        spawn.p += c;
        spawn.r += a;

        return spawn;
    }
}