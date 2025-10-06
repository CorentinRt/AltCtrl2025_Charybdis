using UnityEngine;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Monster : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _smoothTime = 0.1f;

        [SerializeField] private float objectWidth, objectHeight;

        private Vector2 _moveDirection;
        private Vector2 _currentVelocity;
        private Vector2 _targetPosition;

        private bool _isMoving;

        private Rigidbody2D _rb;

        private Vector2 screenBounds;
        // ----- FIELDS ----- //

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

            if (InputManager.Instance == null) return;

            InputManager.Instance.OnLookPressed += OnMonsterMove;
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
                InputManager.Instance.OnLookPressed -= OnMonsterMove;
        }

        public void OnMonsterMove(Vector2 mousePos)
        {
            Debug.Log(mousePos);
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            _targetPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

             // Clamp to screen limits
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
        }

        void FixedUpdate()
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
