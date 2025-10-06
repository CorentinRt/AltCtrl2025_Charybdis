using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
   // ----- FIELDS ----- //
   public static InputManager Instance;

    public event Action<Vector2> OnMovePressed;
    public event Action<Vector2> OnLookPressed;

    public event Action<bool> OnButton0Pressed;
    public event Action<bool> OnButton1Pressed;
    public event Action<bool> OnButton2Pressed;
    public event Action<bool> OnButton3Pressed;
    // ----- FIELDS ----- //

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;
    }

    #region Vector2
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 _moveDirection = context.ReadValue<Vector2>();
            OnMovePressed.Invoke(_moveDirection);
        }
    }

    public void LookPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 _lookDirection = context.ReadValue<Vector2>();
            OnLookPressed.Invoke(_lookDirection);
        }
    }
    #endregion

    #region Buttons
    public void Button0Pressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnButton0Pressed.Invoke(true);
        }
        else if (context.canceled)
        {
            OnButton0Pressed.Invoke(false);
        }
    }

    public void Button1Pressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnButton1Pressed.Invoke(true);
        }
        else if (context.canceled)
        {
            OnButton1Pressed.Invoke(false);
        }
    }

    public void Button2Pressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnButton2Pressed.Invoke(true);
        }
        else if (context.canceled)
        {
            OnButton2Pressed.Invoke(false);
        }
    }

    public void Button3Pressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnButton3Pressed.Invoke(true);
        }
        else if (context.canceled)
        {
            OnButton3Pressed.Invoke(false);
        }
    }
    #endregion
}
