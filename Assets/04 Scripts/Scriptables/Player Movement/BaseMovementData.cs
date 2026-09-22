using UnityEngine;

[CreateAssetMenu(fileName = "Base Movement", menuName = "Scriptable Objects/Player Movement/Base Movement")]
public class BaseMovementData : PlayerMovementData
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float inputMultiplier = 2f;
    
    public override void InputMove(Vector2 input, IMovable movable)
    {
        movable.Move(Vector2.ClampMagnitude(input * inputMultiplier, 1f) * speed);
    }
}
