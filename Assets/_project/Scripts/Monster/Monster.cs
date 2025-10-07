using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Monster : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _smoothTime = 0.1f;
        [SerializeField] private float _typhoonCantMoveTime = 3f;
        [SerializeField] private float _typhoonCooldown = 5f;

        [Header("Screen Limits")]
        [SerializeField] private float objectWidth, objectHeight;

        [Header("References")]
        [SerializeField] private GameObject _monsterTyphoon;

        private Vector2 _moveDirection;
        private Vector2 _currentVelocity;
        private Vector2 _targetPosition;

        private bool _isMoving = true;
        private bool _canTyphoon = true;

        private Rigidbody2D _rb;

        private Vector2 screenBounds;
        // ----- FIELDS ----- //

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

            _monsterTyphoon.SetActive(false);

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnLookPressed += OnMonsterMove;
                InputManager.Instance.OnButton0Pressed += OnMonsterTyphoon;
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnLookPressed -= OnMonsterMove;
                InputManager.Instance.OnButton0Pressed -= OnMonsterTyphoon;
            }
        }

        private void OnMonsterMove(Vector2 mousePos)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            _targetPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

             // Clamp to screen limits
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
        }

        private void OnMonsterTyphoon(bool pressed)
        {
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
            yield return new WaitForSeconds(_typhoonCooldown);
            _canTyphoon = true;
        }

        private IEnumerator StartTyphoonCantMoveTime()
        {
            yield return new WaitForSeconds(_typhoonCantMoveTime);
            _isMoving = true;
        }

        void FixedUpdate()
        {
            if (_isMoving)
            {
                Vector2 currentPosition = _rb.position;
                Vector2 direction = _targetPosition - currentPosition;
                float distance = direction.magnitude;

                if (distance < 0.2f)
                {
                    _rb.MovePosition(_targetPosition);
                    return;
                }

                Vector2 move = direction.normalized * _moveSpeed * Time.fixedDeltaTime;
                _rb.MovePosition(currentPosition + move);
            }
        }
    }
}
