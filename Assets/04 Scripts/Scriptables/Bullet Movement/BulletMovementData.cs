using UnityEngine;

public abstract class BulletMovementData : ScriptableObject
{
    public abstract void Move(IBulletMovable movable);
}