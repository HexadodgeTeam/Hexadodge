using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] private PlayerMovementData data;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 _input;
    
    private void FixedUpdate()
    {
        data.InputMove(_input, this);
    }

    public void Move(Vector2 delta)
    {
        rb.MovePosition(rb.position + delta);
    }
}
