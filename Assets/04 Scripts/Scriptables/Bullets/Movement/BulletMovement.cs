using UnityEngine;

public abstract class BulletMovement : ScriptableObject
{
    public abstract BulletMovementInstance Instance();
}

public abstract class BulletMovementInstance
{
    public abstract void Move(IBulletMovable movable, float time);

    public abstract void ReRoll();
}