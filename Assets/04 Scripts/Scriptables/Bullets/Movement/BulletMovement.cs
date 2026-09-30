using UnityEngine;

public abstract class BulletMovement : ScriptableObject
{
    public abstract void Move(IBulletMovable movable, float time, float random);
}