using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private float stickInputMultiplier = 2f;
    [SerializeField] private float moveBorderWidth = 40f;
    [SerializeField] private float moveBorderSpeed = 3f;
    [SerializeField] private float swipeMultiplier = 1f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Transform ghost;
    
    private Camera _camera;
    private bool _isTouching = false;
    private Vector2 _touchPosition;
    private Vector2 _startTouchPosition;
    private Vector3 _startPosition;
    private Vector2 _moveInput;
    private Control.Type _controlType = Control.Type.Modern;

    private (Func<Vector2, bool>, Vector2)[] _moveBorderChecks;

    private void Awake()
    {
        _moveBorderChecks = new (Func<Vector2, bool>, Vector2)[4];
        _moveBorderChecks[0] = (touchPos => touchPos.y >= Screen.height - moveBorderWidth, Vector2.up);
        _moveBorderChecks[1] = (touchPos => touchPos.y <= moveBorderWidth, Vector2.down);
        _moveBorderChecks[2] = (touchPos => touchPos.x <= moveBorderWidth, Vector2.left);
        _moveBorderChecks[3] = (touchPos => touchPos.x >= Screen.width - moveBorderWidth, Vector2.right);
    }

    private void Start()
    {
        _camera = Camera.main;
    }
    
    private void FixedUpdate()
    {
        if (_controlType == Control.Type.Modern && _isTouching)
        {
            ghost.gameObject.SetActive(true);
            Vector2 borderPush = Vector2.zero;
            
            foreach ((Func<Vector2, bool> check, Vector2 delta) moveBorderCheck in _moveBorderChecks)
            {
                if (moveBorderCheck.check.Invoke(_touchPosition))
                {
                    borderPush += moveBorderCheck.delta;
                }
            }
            
            borderPush.Normalize();
            
            _startPosition += (Vector3)borderPush * (moveBorderSpeed * Time.fixedDeltaTime);
            
            Vector3 touchWorldPosition = _camera.ScreenToWorldPoint(_touchPosition);
            Vector3 touchWorldStartPosition = _camera.ScreenToWorldPoint(_startTouchPosition);
            Vector3 touchDisplacement = (touchWorldPosition - touchWorldStartPosition) * swipeMultiplier;
            Vector3 playerDisplacement = transform.position - _startPosition;
            Vector3 delta = touchDisplacement - playerDisplacement;
            ghost.transform.position = _startPosition + touchDisplacement;
            
            if (delta.magnitude > 0.01f)
            {
                _moveInput = Vector3.ClampMagnitude(delta, 1f);
            }
            else
            {
                _moveInput = Vector2.zero;
            }
        }
        else
        {
            ghost.gameObject.SetActive(false);
        }
        
        float multiplier = _controlType == Control.Type.Classic ? stickInputMultiplier : 1f;
        movement.MoveInput(_moveInput * multiplier);
    }
    
    public void ChangeControlType(int value)
    {
        _controlType = (Control.Type)value;
        playerInput.SwitchCurrentControlScheme(_controlType.GetInputScheme());
        _isTouching = false;
        _moveInput = Vector2.zero;
    }
    
    public void InputCursor(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TouchState touch = context.ReadValue<TouchState>();

            if (touch.phase == TouchPhase.Began)
            {
                _isTouching = true;
                _startPosition = transform.position;
                _startTouchPosition = touch.startPosition;
            }

            if (touch.phase.IsActive())
            {
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
}