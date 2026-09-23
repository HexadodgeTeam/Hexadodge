using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] [EnumButtons] private Control.Type controlType;
    [SerializeField] private float stickInputMultiplier = 2f;
    [SerializeField] private PlayerMovementData data;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerInput;

    private Camera _camera;
    private bool _isTouching = false;
    private Vector2 _touchPosition;
    private Vector2 _startTouchPosition;
    private Vector3 _startPosition;
    private Vector2 _moveInput;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (controlType == Control.Type.Modern && _isTouching)
        {
            Vector3 touchWorldPosition = _camera.ScreenToWorldPoint(_touchPosition);
            Vector3 touchWorldStartPosition = _camera.ScreenToWorldPoint(_startTouchPosition);
            Vector3 touchDisplacement = touchWorldPosition - touchWorldStartPosition;
            Vector3 playerDisplacement = transform.position - _startPosition;
            Vector3 delta = touchDisplacement - playerDisplacement;
            if (delta.magnitude > 0.1f)
            {
                _moveInput = delta.normalized;
            }
            else
            {
                _moveInput = Vector2.zero;
            }
        }
        
        float multiplier = controlType == Control.Type.Classic ? stickInputMultiplier : 1f;
        data.InputMove(_moveInput * multiplier, this);
    }

    public void ChangeControlType(int value)
    {
        controlType = (Control.Type)value;
        playerInput.SwitchCurrentControlScheme(controlType.GetInputScheme());
        _isTouching = false;
        _moveInput = Vector2.zero;
    }

    public void InputMove(InputAction.CallbackContext context)
    {
        print("fw");
        if (context.performed)
        {
            print("s");
            _moveInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            _moveInput = Vector2.zero;
        }
    }

    public void InputCursor(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TouchState touch = context.ReadValue<TouchState>();

            if (touch.phase == TouchPhase.Began)
            {
                _startPosition = transform.position;
                _startTouchPosition = touch.startPosition;
            }

            if (touch.phase.IsActive())
            {
                _isTouching = true;
                _touchPosition = touch.position;
            }
            else if (touch.phase.IsEndedOrCanceled())
            {
                _isTouching = false;
                _moveInput = Vector2.zero;
            }
        }
        else if (context.canceled)
        {
            _isTouching = false;
            _moveInput = Vector2.zero;
        }
    }

    public void Move(Vector2 delta)
    {
        rb.MovePosition(rb.position + delta * Time.fixedDeltaTime);
    }
}
