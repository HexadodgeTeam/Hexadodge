using UnityEngine;

[CreateAssetMenu(fileName = "Base Movement", menuName = "Scriptable Objects/Player/Movement/Base Movement")]
public class BaseMovementData : PlayerMovementData
{
    [SerializeField] private float speed = 2f;
    
    public override void InputMove(Vector2 input, IMovable movable)
    {
        movable.Move(Vector2.ClampMagnitude(input, 1f) * speed);
    }
}
