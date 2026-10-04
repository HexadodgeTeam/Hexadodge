using UnityEngine;

[CreateAssetMenu(fileName = "Straight Line", menuName = "Scriptable Objects/Bullets/Movement/Straight Line")]
public class BMStraightLine : BulletMovement
{
    [SerializeField] internal FloatCurve speed = 1f;

    public override BulletMovementInstance Instance() => new BMStraightLineInstance(this);
}

public class BMStraightLineInstance : BulletMovementInstance
{
    private FloatCurveInstance speed;

    public BMStraightLineInstance(BMStraightLine data)
    {
        speed = data.speed.Instance();
    }
    
    public override void Move(IBulletMovable movable, float time)
    {
        movable.Move(movable.Forward * speed.Eval(time));
    }

    public override void ReRoll()
    {
        speed.ReRoll();
    }
}
