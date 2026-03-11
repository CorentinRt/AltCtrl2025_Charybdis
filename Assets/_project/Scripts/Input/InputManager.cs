using AltCtrl.Charybdis;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
   // ----- FIELDS ----- //
   public static InputManager Instance;

    [Header("References")]
    [SerializeField] private ArduinoReader _arduinoReader;

    [Header("Tutorials")]
    [SerializeField] private bool _detectGodInputs = true;
    [SerializeField] private bool _detectHumanInputs = true;

    public event Action<Vector2> OnMoveMonsterPressed;
    public event Action<Vector2> OnMoveTurboPressed;

    public event Action<bool> OnTotem1Pressed;
    public event Action<bool> OnTotem2Pressed;
    public event Action<bool> OnTotem3Pressed;
    public event Action<bool> OnWindPressed;

    public event Action<bool> OnNextMicrophonePressed;
    public event Action<bool> OnNextLanguagePressed;

    public event Action<bool> OnPausePressed;

    public event Action<int> OnMoveShipRotatorPressed;
    public event Action<int> OnMoveRadioRotatorPressed;

    // Microphone
    private string _currentMicrophone = "";
    private int _currentMicrophoneIndex = -1;
    private int _nbrMicrophoneDevices = -1;

    private bool _lastWindState = true;
    // ----- FIELDS ----- //

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transform.parent != null) transform.parent = null;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (_arduinoReader != null)
        {
            _arduinoReader.OnRotator1Move += MoveShipRotatorPressed;
            _arduinoReader.OnRotator2Move += MoveRadioRotatorPressed;
        }

        GetCurrentOrFirstMicrophone();

        foreach (var d in InputSystem.devices)
        {
            Debug.Log(d);
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

    public void SetDetectGodInput(bool enable)
    {
        _detectGodInputs = enable;
    }

    public void SetDetectHumanInput(bool enable)
    {
        _detectHumanInputs = enable;
    }

    private void Update()
    {
        //Debug.Log($"Cards nbr : {Joystick.all.Count}");

        // Carte 1 (Joystick 1)
        if (Joystick.all.Count > 0) // ou Joystick.all.Count si c'est un joystick
        {
            var card1 = Joystick.all[0]; // première carte
            Vector2 move1 = card1.stick.ReadValue(); // stick gauche
            if (move1 !=  Vector2.zero)
            {
                Debug.Log($"move 1 : {move1}");
                OnMoveTurboPressed?.Invoke(move1);
            }

            for (int i = 0; i < card1.allControls.Count; i++)
            {
                var control = card1.allControls[i];
                if (control is ButtonControl button && button.isPressed)
                {
                    Debug.Log($"Button pressed: {control.name} (index {i})");
                }
            }

        }

        // Carte 2 (Joystick 2)
        if (Joystick.all.Count > 1)
        {
            var card2 = Joystick.all[1];
            //Debug.Log(card2);
            Vector2 move2 = card2.stick.ReadValue();
            if (move2 != Vector2.zero)
            {
                Debug.Log($"move 2 : {move2}"); 
                OnMoveMonsterPressed?.Invoke(move2);
            }
        }
    }

    #region Microphone
    public string GetCurrentOrFirstMicrophone()
    {
        if (_currentMicrophone == "")
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("Aucun microphone d�tect� !");
                return "";
            }

            _nbrMicrophoneDevices = Microphone.devices.Length;
            _currentMicrophoneIndex = 0;
            _currentMicrophone = Microphone.devices[_currentMicrophoneIndex];
        }

        return _currentMicrophone;
    }

    public void SetNextMicrophone()
    {
        _currentMicrophoneIndex++;
        if (_currentMicrophoneIndex > _nbrMicrophoneDevices - 1) _currentMicrophoneIndex = 0;

        _currentMicrophone = Microphone.devices[_currentMicrophoneIndex];
    }
    #endregion

    #region Arduino
    public void MoveShipRotatorPressed(int direction)
    {
        if (!_detectHumanInputs) return;

        Debug.Log($"Move ship in direction {direction}");
        OnMoveShipRotatorPressed?.Invoke(direction);
    }

    public void MoveRadioRotatorPressed(int direction)
    {
        if (!_detectHumanInputs) return;

        Debug.Log($"Move radio in direction {direction}");
        OnMoveRadioRotatorPressed?.Invoke(direction);
    }
    #endregion

    #region Vector2
    public void MoveMonsterPressed(InputAction.CallbackContext context)
    {
        return;

        /*
        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();

            // seuil pour filtrer les micro-mouvements
            if (moveDirection.magnitude < 0.1f)
                moveDirection = Vector2.zero;

            Debug.Log($"Move : {moveDirection.ToString()}");

            OnMoveMonsterPressed?.Invoke(moveDirection);
        }
        */
    }
    
    public void MoveMonsterTempPressed(InputAction.CallbackContext context)
    {
        return;

        /*
        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();

            // seuil pour filtrer les micro-mouvements
            if (moveDirection.magnitude < 0.1f)
                moveDirection = Vector2.zero;

            OnMoveMonsterTempPressed?.Invoke(moveDirection);
        }
        */
    }

    public void MoveTurboPressed(InputAction.CallbackContext context)
    {
        return;

        /*
        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();

            // seuil pour filtrer les micro-mouvements
            if (moveDirection.magnitude < 0.1f)
                moveDirection = Vector2.zero;

            OnMoveTurboPressed?.Invoke(moveDirection);
        }
        */
    }

    public void MoveRadioKeyboardPressed(InputAction.CallbackContext context)
    {
        if (!_detectHumanInputs) return;

        if (context.performed)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();
            //Debug.Log($"Radio : {moveDirection}");
            OnMoveRadioRotatorPressed?.Invoke((int)moveDirection.x);
        }
    }

    public void MoveGouvernailKeyboardPressed(InputAction.CallbackContext context)
    {
        if (!_detectHumanInputs) return;

        if (context.performed)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();
            //Debug.Log($"Gouvernail : {moveDirection}");
            OnMoveShipRotatorPressed?.Invoke((int)moveDirection.x);
        }
    }
    #endregion

    #region Buttons
    public void Totem1Pressed(InputAction.CallbackContext context)
    {
        if (!_detectGodInputs) return;

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
        if (!_detectGodInputs) return;

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
        if (!_detectGodInputs) return;

        if (context.performed)
        {
            OnTotem3Pressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnTotem3Pressed?.Invoke(false);
        }
    }

    public void WindPressed(InputAction.CallbackContext context)
    {
        if (!_detectGodInputs) return;

        if (context.performed)
        {
            OnWindPressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnWindPressed?.Invoke(false);
        }
    }

    public void NextMicrophonePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnNextMicrophonePressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnNextMicrophonePressed?.Invoke(false);
        }
    }

    public void NextLanguagePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnNextLanguagePressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnNextLanguagePressed?.Invoke(false);
        }
    }

    public void PausePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPausePressed?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnPausePressed?.Invoke(false);
        }
    }
    #endregion
}
