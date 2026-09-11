using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] private PlayerMovementData data;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 _moveInput;

    private void Update()
    {
        data.InputMove(_moveInput, this);
    }

    public void InputMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            _moveInput = Vector2.zero;
        }
    }

    public void Move(Vector2 delta)
    {
        rb.linearVelocity = delta;
    }
}
