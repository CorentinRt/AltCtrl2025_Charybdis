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
        [SerializeField] private bool _isInMenu = false;

        [Header("Screen Limits")]
        [SerializeField] private float objectWidth, objectHeight;

        [Header("References")]
        [SerializeField] private MonsterTarget _target;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _monsterVisuals;

        [Header("Trajectory")]
        [SerializeField] private LineRenderer _trajectoryLine;
        [SerializeField] private int _predictionSteps = 50;
        [SerializeField] private float _timeStep = 0.1f;

        private Vector2 _moveDirection;
        private Vector2 _currentVelocity;
        private Vector2 _targetPosition;

        private float _currentAngularVelocity;

        private bool _isMoving = true;
        private bool _canTyphoon = true;
        private bool _isTeleporting = false;
        private bool _canTeleport = true;

        private Rigidbody2D _rb;

        private Vector2 screenBounds;

        private bool _inputEnabled = true;

        private Vector3[] _trajectoryPointsBuffer;

        // ----- FIELDS ----- //

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            /*
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePhaseChanged += Instance_OnGamePhaseChanged;
            } 

            if (_isInMenu)
            {
                StartCoroutine(StartTyphoonCooldown());
            }
            */
        }

        /*
        private void Instance_OnGamePhaseChanged(GameManager.GAME_PHASES obj)
        {
            if (obj == GameManager.GAME_PHASES.IN_GAME)
            {
                StartCoroutine(StartTyphoonCooldown());
            }
        }
        */

        private void OnDestroy()
        {
            /*
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePhaseChanged -= Instance_OnGamePhaseChanged;
            }
            */
        }

        private void MoveMonsterTowardsTarget(float deltaTime)
        {
            if (_target == null)
                return;

            Vector2 currentPosition = _rb.position;
            Vector2 targetPosition = _target.GetComponent<Rigidbody2D>().position; 

            Vector2 direction = targetPosition - currentPosition;
            float distance = direction.magnitude;

            if (distance < 0.05f)
            {
                _rb.MovePosition(targetPosition);
                return;
            }

            Vector2 move = direction.normalized * _monsterData.MoveSpeed * deltaTime;
            Vector2 newPosition = currentPosition + move;

            _rb.MovePosition(newPosition);
        }

        private void OnMonsterTyphoon(bool pressed)
        {
            //Debug.Log("typhoon");

            if (!_inputEnabled)
                return;

            if (pressed && _canTyphoon)
            {
                _isMoving = false;
                _canTyphoon = false;

                _animator.SetTrigger("ThyphonSpawn");

                if (PoolManager.Instance != null)
                {
                    GameObject newTyphoonGO = PoolManager.Instance.ActivateTyphoon(transform.position, transform.rotation);
                    MonsterTyphoon newTyphoon = newTyphoonGO.GetComponent<MonsterTyphoon>();
                    newTyphoon.ActivateTyphon();
                }
                else
                {
                    Debug.LogError("No pool manager found in scene");
                }
                

                StartCoroutine(StartTyphoonCooldown());
                StartCoroutine(StartTyphoonCantMoveTime());
            }
        }

        private void OnMonsterTyphoonIndication()
        {
            // ----- AUDIO ----- //
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("Typhoon_Warning");
            // ----- AUDIO ----- //

            _animator.SetBool("Indicator", true);
            StartCoroutine(WaitAndDesacTyphoonIndicator());

        }

        private IEnumerator WaitAndDesacTyphoonIndicator()
        {
            yield return new WaitForSeconds(_monsterData.IndicationAnimTime);
            _animator.SetBool("Indicator", false);
        }

        private IEnumerator StartTyphoonCooldown()
        {
            yield return new WaitForSeconds(_monsterData.TyphoonCooldown - _monsterData.IndicationBeforeTyphoon);
            OnMonsterTyphoonIndication();
            yield return new WaitForSeconds(_monsterData.IndicationBeforeTyphoon);
            _canTyphoon = true;
            OnMonsterTyphoon(true);
        }

        private IEnumerator StartTyphoonCantMoveTime()
        {
            yield return new WaitForSeconds(_monsterData.TyphoonCantMoveTime);
            _isMoving = true;
        }

        private void Update()
        {
            PredictTrajectory();
        }
        void FixedUpdate()
        {
            //if (!_isMoving || !_inputEnabled) return;

            MoveMonsterTowardsTarget(Time.fixedDeltaTime);
        }

        #region Teleport
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
            Debug.Log("Teleport cooldown");
            yield return new WaitForSeconds(_monsterData.TeleportCooldown);
            _canTeleport = true;
        }
        #endregion

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

        public void SetUnderground(bool underground)
        {
            _animator.SetBool("Underground", underground);
        }

        #region Trajectory
        private void PredictTrajectory()
        {
            /*
            Vector2 startPos = _rb.position;
            Vector2 velocity = _rb.linearVelocity;
            float angularVel = _rb.angularVelocity;

            if (_trajectoryPointsBuffer == null || _trajectoryPointsBuffer.Length != _predictionSteps)
            {
                _trajectoryPointsBuffer = new Vector3[_predictionSteps];
            }

            float rotRadPerStep = angularVel * Mathf.Deg2Rad * _timeStep;

            Vector2 pos = startPos;
            Vector2 dir = velocity.normalized;

            for (int i = 0; i < _predictionSteps; i++)
            {
                pos += dir * velocity.magnitude * _timeStep;

                float cos = Mathf.Cos(rotRadPerStep);
                float sin = Mathf.Sin(rotRadPerStep);

                dir = new Vector2(
                    dir.x * cos - dir.y * sin,
                    dir.x * sin + dir.y * cos
                );

                _trajectoryPointsBuffer[i] = pos;
            }

            _trajectoryLine.positionCount = _predictionSteps;
            _trajectoryLine.SetPositions(_trajectoryPointsBuffer);
            */
        }

        #endregion
    }
}
