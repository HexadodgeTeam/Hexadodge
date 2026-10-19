using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] private PlayerMovementData data;
    [SerializeField] private Rigidbody2D rb;

    public void MoveInput(Vector2 input)
    {
        data.InputMove(input, this);
    }

    public void Move(Vector2 delta)
    {
        rb.MovePosition(rb.position + delta * Game.DeltaTime);
    }
}
