using CREMOT.GameplayUtilities;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MonsterTarget : GenericSingleton<MonsterTarget>
    {
        // ----- FIELDS ----- //
        [Header("Data")]
        [SerializeField] private SO_MonsterTargetData _monsterTargetData;

        [Header("Screen Limits")]
        [SerializeField] private float _objectWidth, _objectHeight;

        private Vector2 _screenBounds;

        private Vector2 _moveDirection;
        private Rigidbody2D _rb;

        private bool _inputEnabled = false;
        // ----- FIELDS ----- //

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMoveMonsterPressed += OnMonsterJoystickMove;
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMoveMonsterPressed -= OnMonsterJoystickMove;
            }
        }

        private void FixedUpdate()
        {
            if (!_inputEnabled) return;

            MoveTarget(Time.fixedDeltaTime);
        }

        private void MoveTarget(float deltaTime)
        {
            Vector2 move = _moveDirection.normalized * _monsterTargetData.MoveSpeed * deltaTime;
            Vector2 newPosition = _rb.position + move;

            newPosition.x = Mathf.Clamp(newPosition.x, -_screenBounds.x + _objectWidth, _screenBounds.x - _objectWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, -_screenBounds.y + _objectHeight, _screenBounds.y - _objectHeight);

            _rb.MovePosition(newPosition);
        }

        private void OnMonsterJoystickMove(Vector2 moveDirection)
        {
            _moveDirection = moveDirection;
        }

        public void SetEnableMonsterTargetInput(bool enabled)
        {
            _inputEnabled = enabled;
        }
    }
}
