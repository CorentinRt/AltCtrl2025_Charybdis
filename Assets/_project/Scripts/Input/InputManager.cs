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
    }

    private void OnDestroy()
    {
        if (_arduinoReader != null)
        {
            _arduinoReader.OnRotator1Move -= MoveShipRotatorPressed;
            _arduinoReader.OnRotator2Move -= MoveRadioRotatorPressed;
        }
    }

    private void Update()
    {
        float xMonster = 0f;
        float yMonster = 0f;

        if (Input.GetKey(KeyCode.D))
        {
            xMonster = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            xMonster = -1f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            yMonster = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            yMonster = -1f;
        }


        Vector2 dirMonster = new Vector2 (xMonster, yMonster);

        OnMoveMonsterPressed?.Invoke(dirMonster);

        float yHuman = 0f;

        if (Input.GetKey(KeyCode.I))
        {
            yHuman = 1f;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            yHuman = -1f;
        }

        Vector2 dirHuman = new Vector2 (0f, yHuman);

        OnMoveTurboPressed?.Invoke(dirHuman);
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
        Debug.Log($"Move ship in direction {direction}");
        OnMoveShipRotatorPressed?.Invoke(direction);
    }

    public void MoveRadioRotatorPressed(int direction)
    {
        Debug.Log($"Move radio in direction {direction}");
        OnMoveRadioRotatorPressed?.Invoke(direction);
    }
    #endregion

    #region Vector2
    public void MoveMonsterPressed(InputAction.CallbackContext context)
    {
        return;

        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();

            // seuil pour filtrer les micro-mouvements
            if (moveDirection.magnitude < 0.1f)
                moveDirection = Vector2.zero;

            Debug.Log($"Move : {moveDirection.ToString()}");

            OnMoveMonsterPressed?.Invoke(moveDirection);
        }
    }
    
    public void MoveMonsterTempPressed(InputAction.CallbackContext context)
    {
        return;

        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();

            // seuil pour filtrer les micro-mouvements
            if (moveDirection.magnitude < 0.1f)
                moveDirection = Vector2.zero;

            OnMoveMonsterTempPressed?.Invoke(moveDirection);
        }
    }

    public void MoveTurboPressed(InputAction.CallbackContext context)
    {
        return;

        if (context.performed || context.canceled)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();

            // seuil pour filtrer les micro-mouvements
            if (moveDirection.magnitude < 0.1f)
                moveDirection = Vector2.zero;

            OnMoveTurboPressed?.Invoke(moveDirection);
        }
    }

    public void MoveRadioKeyboardPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();
            //Debug.Log($"Radio : {moveDirection}");
            OnMoveRadioRotatorPressed?.Invoke((int)moveDirection.x);
        }
    }

    public void MoveGouvernailKeyboardPressed(InputAction.CallbackContext context)
    {
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

    public void WindPressed(InputAction.CallbackContext context)
    {
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
