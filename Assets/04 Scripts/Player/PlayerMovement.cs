using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] private Rigidbody2D rb;
    
    private PlayerMovementData _data;

    public void Init(PlayerMovementData data)
    {
        _data = data;
    }
    
    public void MoveInput(Vector2 input)
    {
        _data.InputMove(input, this);
    }

    public void Move(Vector2 delta)
    {
        rb.MovePosition(rb.position + delta * Game.DeltaTime);
    }
}
