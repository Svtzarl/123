using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour, PlayerInput.IPlayerActions
{
    private PlayerInput _inputActions;
    public Vector2 MoveInput { get; private set; }
    private float _lastJumpPressTime = float.NegativeInfinity;

    // Свойства для доступа
    public float LastJumpPressTime => _lastJumpPressTime;


    private void Awake()
    {
        _inputActions = new PlayerInput();
        _inputActions.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _lastJumpPressTime = Time.time;

            Debug.Log("Space");
        }
        else if (context.canceled)
        {
            _lastJumpPressTime = float.NegativeInfinity;
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void ResetLastJumpPressTime()
    {
        _lastJumpPressTime = float.NegativeInfinity;
    }
}
