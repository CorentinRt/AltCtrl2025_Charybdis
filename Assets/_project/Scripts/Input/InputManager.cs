using AltCtrl.Charybdis;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
   // ----- FIELDS ----- //
   public static InputManager Instance;

    [Header("References")]
    [SerializeField] private ArduinoReader _arduinoReader;

    public event Action<Vector2> OnMoveMonsterPressed;
    public event Action<Vector2> OnMoveMonsterTempPressed;
    public event Action<Vector2> OnMoveTurboPressed;

    public event Action<bool> OnTotem1Pressed;
    public event Action<bool> OnTotem2Pressed;
    public event Action<bool> OnTotem3Pressed;
    public event Action<bool> OnTyphonPressed;

    public event Action<int> OnMoveShipRotatorPressed;
    public event Action<int> OnMoveRadioRotatorPressed;
    // ----- FIELDS ----- //

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;
    }

    private void Start()
    {
        if (_arduinoReader != null)
        {
            _arduinoReader.OnRotator1Move += MoveShipRotatorPressed;
            _arduinoReader.OnRotator2Move += MoveRadioRotatorPressed;
        }
    }

    private void OnDestroy()
    {
        if (_arduinoReader != null)
        {
            _arduinoReader.OnRotator1Move -= MoveShipRotatorPressed;
            _arduinoReader.OnRotator2Move -= MoveRadioRotatorPressed;
        }
    }

    #region Arduino
    public void MoveShipRotatorPressed(int direction)
    {
        //Debug.Log($"Move ship in direction {direction}");
        OnMoveShipRotatorPressed?.Invoke(direction);
    }

    public void MoveRadioRotatorPressed(int direction)
    {
        //Debug.Log($"Move radio in direction {direction}");
        OnMoveRadioRotatorPressed?.Invoke(direction);
    }
    #endregion

    #region Joysticks
    public void MoveMonsterPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();
            OnMoveMonsterPressed?.Invoke(moveDirection);
        }
    }
    
    public void MoveMonsterTempPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();
            OnMoveMonsterTempPressed?.Invoke(moveDirection);
        }
    }

    public void MoveTurboPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();
            OnMoveTurboPressed?.Invoke(moveDirection);
        }
    }
    #endregion

    #region Buttons
    public void Totem1Pressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnTotem1Pressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnTotem1Pressed?.Invoke(false);
        }
    }

    public void Totem2Pressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnTotem2Pressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnTotem2Pressed?.Invoke(false);
        }
    }

    public void Totem3Pressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnTotem3Pressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnTotem3Pressed?.Invoke(false);
        }
    }

    public void TyphonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnTyphonPressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnTyphonPressed?.Invoke(false);
        }
    }
    #endregion
}
