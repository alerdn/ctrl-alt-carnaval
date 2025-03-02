using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

public enum ControllerMode
{
    Gameplay,
    UI
}

[CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
public class InputReader : ScriptableObject, IPlayerActions, IUIActions
{
    public event Action OnPauseEvent;

    public Vector2 MovementValue { get; private set; }

    private Controls _controls;
    private ControllerMode _controllerMode;

    private void OnEnable()
    {
        _controls ??= new Controls();
        _controls.Player.SetCallbacks(this);
    }

    public void SetControllerMode(ControllerMode mode)
    {
        _controllerMode = mode;
        switch (_controllerMode)
        {
            case ControllerMode.Gameplay:
                _controls.Player.Enable();
                _controls.UI.Disable();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case ControllerMode.UI:
                _controls.Player.Disable();
                _controls.UI.Enable();
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnTogglePause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        OnPauseEvent?.Invoke();
    }
}
