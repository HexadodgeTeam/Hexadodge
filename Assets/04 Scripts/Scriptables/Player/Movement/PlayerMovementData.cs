using UnityEngine;

public abstract class PlayerMovementData : ScriptableObject
{
    public abstract void InputMove(Vector2 input, IMovable movable);
}