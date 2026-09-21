using UnityEngine;

public interface IBulletMovable
{
    public Vector2 Forward { get; }

    public void Move(Vector2 delta);
}