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
        [SerializeField] private float _objectWidth = 0.5f; 
        [SerializeField] private float _objectHeight;

        private Vector2 _screenBounds;

        private Vector2 _gameplayMinZone;
        private Vector2 _gameplayMaxZone;

        private Vector3 _centerGameplayZone;

        private Vector2 _moveDirection;
        private Rigidbody2D _rb;

        private bool _inputEnabled = true;
        // ----- FIELDS ----- //

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            Camera camera = Camera.main;

            float leftViewport = ViewportAdjusterManager.LeftLimit;

            Vector3 bottomLeft = camera.ViewportToWorldPoint(
                new Vector3(leftViewport, 0f, camera.nearClipPlane)
            );

            Vector3 topRight = camera.ViewportToWorldPoint(
                new Vector3(1f, 1f, camera.nearClipPlane)
            );

            _gameplayMinZone = bottomLeft;
            _gameplayMaxZone = topRight;

            _screenBounds.x = (_gameplayMaxZone.x - _gameplayMinZone.x) * 0.5f;
            _screenBounds.y = (_gameplayMaxZone.y - _gameplayMinZone.y) * 0.5f;

            _centerGameplayZone = (_gameplayMaxZone + _gameplayMinZone) / 2f;

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

            newPosition.x = Mathf.Clamp(newPosition.x, _gameplayMinZone.x + _objectWidth, _gameplayMaxZone.x - _objectWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, _gameplayMinZone.y + _objectHeight, _gameplayMaxZone.y - _objectHeight);

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
