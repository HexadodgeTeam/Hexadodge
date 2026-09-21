using UnityEngine;

[CreateAssetMenu(fileName = "Straight Line", menuName = "Scriptable Objects/Bullet Movement/Straight Line")]
public class StraightLineMovementData : BulletMovementData
{
    public float speed = 100f;

    public override void Move(IBulletMovable movable)
    {
        movable.Move(movable.Forward * speed);
    }
}
