using UnityEngine;

[CreateAssetMenu(fileName = "Base Movement", menuName = "Scriptable Objects/Movement/Base Movement")]
public class BaseMovementData : PlayerMovementData
{
    [SerializeField] private float speed = 2f;
    
    public override void InputMove(Vector2 input, IMovable movable)
    {
        float totalSpeed = speed * Time.deltaTime;
            
        movable.Move(Vector2.ClampMagnitude(input, 1f) * (totalSpeed * Time.fixedDeltaTime));
    }
}
