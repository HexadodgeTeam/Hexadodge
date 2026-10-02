using UnityEngine;

[CreateAssetMenu(fileName = "Straight Line", menuName = "Scriptable Objects/Bullets/Movement/Straight Line")]
public class BMStraightLine : BulletMovement
{
    [SerializeField] private FloatCurve speed = 1f;

    public override void Move(IBulletMovable movable, float time, float random)
    {
        movable.Move(movable.Forward * speed.Eval(time, random));
    }
}
