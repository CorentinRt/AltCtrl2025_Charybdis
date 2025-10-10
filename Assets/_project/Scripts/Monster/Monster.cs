using CREMOT.GameplayUtilities;
using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Monster : GenericSingleton<Monster>
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private SO_MonsterData _monsterData;

        [Header("Controls")]
        [SerializeField] private bool _useJoystick = false;

        [Header("Screen Limits")]
        [SerializeField] private float objectWidth, objectHeight;

        [Header("References")]
        [SerializeField] private GameObject _monsterTyphoon;

        private Vector2 _moveDirection;
        private Vector2 _currentVelocity;
        private Vector2 _targetPosition;

        private bool _isMoving = true;
        private bool _canTyphoon = true;
        private bool _isTeleporting = false;
        private bool _canTeleport = true;

        private Rigidbody2D _rb;

        private Vector2 screenBounds;

        private bool _inputEnabled;

        // ----- FIELDS ----- //

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

            _monsterTyphoon.SetActive(false);

            if (InputManager.Instance != null)
            {
                if (_useJoystick)
                {
                    InputManager.Instance.OnMoveMonsterPressed += OnMonsterJoystickMove;
                }
                else
                {
                    InputManager.Instance.OnMoveMonsterTempPressed += OnMonsterMouseMove;
                }

                InputManager.Instance.OnTyphonPressed += OnMonsterTyphoon;
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                if (_useJoystick)
                {
                    InputManager.Instance.OnMoveMonsterPressed -= OnMonsterJoystickMove;
                }
                else
                {
                    InputManager.Instance.OnMoveMonsterTempPressed -= OnMonsterMouseMove;
                }

                InputManager.Instance.OnTyphonPressed -= OnMonsterTyphoon;
            }
        }

        private void OnMonsterMouseMove(Vector2 mousePos)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            _targetPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // Clamp to screen limits
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
        }

        private void OnMonsterJoystickMove(Vector2 moveDirection)
        {
            _moveDirection = moveDirection;
        }

        private void OnMonsterTyphoon(bool pressed)
        {
            if (!_inputEnabled)
                return;

            if (pressed & _isMoving && _canTyphoon)
            {
                _isMoving = false;
                _canTyphoon = false;

                _monsterTyphoon.transform.position = transform.position;
                _monsterTyphoon.SetActive(true);

                StartCoroutine(StartTyphoonCooldown());
                StartCoroutine(StartTyphoonCantMoveTime());
            }
        }

        private IEnumerator StartTyphoonCooldown()
        {
            yield return new WaitForSeconds(_monsterData.TyphoonCooldown);
            _canTyphoon = true;
        }

        private IEnumerator StartTyphoonCantMoveTime()
        {
            yield return new WaitForSeconds(_monsterData.TyphoonCantMoveTime);
            _isMoving = true;
        }

        void FixedUpdate()
        {
            if (!_isMoving || !_inputEnabled)
                return;

            if (_useJoystick)
            {
                Vector2 move = _moveDirection.normalized * _monsterData.MoveSpeed * Time.fixedDeltaTime;
                Vector2 newPosition = _rb.position + move;

                newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
                newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);

                _rb.MovePosition(newPosition);
            }
            else
            {
                Vector2 currentPosition = _rb.position;
                Vector2 direction = _targetPosition - currentPosition;
                float distance = direction.magnitude;

                if (distance < 0.2f)
                {
                    _rb.MovePosition(_targetPosition);
                    return;
                }

                Vector2 move = direction.normalized * _monsterData.MoveSpeed * Time.fixedDeltaTime;
                _rb.MovePosition(currentPosition + move);
            }
        }


        public void SetTeleporting(bool isTeleporting)
        {
            _isTeleporting = isTeleporting;

            if (_isTeleporting)
            {
                _canTeleport = false;
                _isMoving = false;
            }
            else
            {
                StartCoroutine(TeleportCooldown());
            }
        }

        private IEnumerator TeleportCooldown()
        {
            yield return new WaitForSeconds(_monsterData.TeleportCooldown);
            _canTeleport = true;
        }

        public void SetCanMove(bool canMove)
        {
            _isMoving = canMove;
        }

        public void SetEnableMonsterInput(bool enabled)
        {
            _inputEnabled = enabled;
        }

        public bool IsTeleporting() { return _isTeleporting; }

        public bool CanTeleport() { return _canTeleport; }
    }
}
