using CREMOT.GameplayUtilities;
using DG.Tweening;
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

        [Header("References")]
        [SerializeField] private MonsterTarget _target;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _monsterVisuals;
        [SerializeField] private GameObject _warningVisuals;
        [SerializeField] private Transform _visualAnchor;

        private bool _isMoving = true;
        private bool _canTyphoon = false;

        private Rigidbody2D _rb;

        private bool _canMove = true;

        private Tween _spawnTween;

        private bool _isUnderwater = true;
        // ----- FIELDS ----- //

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (GameManager.Instance != null)
                GameManager.Instance.OnGamePhaseChanged += Instance_OnGamePhaseChanged;

            if (_isInMenu)
                StartCoroutine(StartTyphoonCooldown());

            PlaySpawnAnimation();

            _warningVisuals.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGamePhaseChanged -= Instance_OnGamePhaseChanged;
        }

        private void Instance_OnGamePhaseChanged(GameManager.GAME_PHASES obj)
        {
            if (obj == GameManager.GAME_PHASES.IN_GAME)
                StartCoroutine(StartTyphoonCooldown());
        }

        void FixedUpdate()
        {
            if (!_canMove || !_isMoving) return;

            MoveMonsterTowardsTarget(Time.fixedDeltaTime);
        }

        private void MoveMonsterTowardsTarget(float deltaTime)
        {
            if (_target == null)
                return;

            // add _monsterVisuals look at target

            Vector2 currentPosition = _rb.position;
            Vector2 targetPosition = _target.GetComponent<Rigidbody2D>().position;

            Vector2 toTarget = targetPosition - currentPosition;
            float distance = toTarget.magnitude;

            if (distance < 0.05f)
            {
                _rb.MovePosition(targetPosition);
                OnMonsterTyphoon();
                return;
            }

            // Look at target
            if (toTarget.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg + 90f;

                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

                _monsterVisuals.transform.rotation = Quaternion.Slerp(
                    _monsterVisuals.transform.rotation,
                    targetRotation,
                    _monsterData.RotationSpeed * deltaTime
                );
            }

            // Check underwater anim
            if (distance < _monsterData.TargetUnderwaterMinDistance && _isUnderwater)
            {
                _isUnderwater = false;
                _animator.SetBool("Surface", true);
            }
            else if (distance > _monsterData.TargetUnderwaterMinDistance && !_isUnderwater)
            {
                _isUnderwater = true;
                _animator.SetBool("Surface", false);
            }

            Vector2 dirToTarget = toTarget.normalized;

            float moveSpeed = TweakableOptionsManager.Exist
                ? TweakableOptionsManager.Instance.GetMonsterMaxSpeed()
                : _monsterData.MoveSpeed;

            Vector2 finalDirection = dirToTarget;

            if (distance < _monsterData.ArcStartDistance)
            {
                Vector2 perpendicular = new Vector2(-dirToTarget.y, dirToTarget.x);
                float arcStrength = 1f - (distance / _monsterData.ArcStartDistance);
                finalDirection = (dirToTarget + perpendicular * arcStrength).normalized;
            }

            Vector2 move = finalDirection * moveSpeed * deltaTime;
            _rb.MovePosition(currentPosition + move);
        }


        private void OnMonsterTyphoon()
        {
            //Debug.Log("typhoon");

            if (!_canMove) return;

            if (_canTyphoon)
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

                OnMonsterTyphoonStopIndication();
            }
        }

        #region Typhoon Indication
        private void OnMonsterTyphoonIndication()
        {
            // ----- AUDIO ----- //
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("Typhoon_Warning");
            // ----- AUDIO ----- //

            //_animator.SetBool("Indicator", true);
            _warningVisuals.SetActive(true);

        }

        private void OnMonsterTyphoonStopIndication()
        {
            //_animator.SetBool("Indicator", false);
            _warningVisuals.SetActive(false);
        }
        #endregion

        private IEnumerator StartTyphoonCooldown()
        {
            yield return new WaitForSeconds(_monsterData.TyphoonCooldown - _monsterData.IndicationBeforeTyphoon);

            OnMonsterTyphoonIndication();

            yield return new WaitForSeconds(_monsterData.IndicationBeforeTyphoon);

            _canTyphoon = true;
        }
        private IEnumerator StartTyphoonCantMoveTime()
        {
            yield return new WaitForSeconds(_monsterData.TyphoonCantMoveTime);
            _isMoving = true;
        }

        public void SetEnableMonsterMovement(bool enabled)
        {
            _canMove = enabled;
        }

        #region Spawn Dev Anim
        private void PlaySpawnAnimation()
        {
            StopSpawnAnimation();

            _visualAnchor.localScale = Vector3.zero;

            _spawnTween = _visualAnchor.DOScale(1f, _monsterData.DurationSpawn).SetEase(_monsterData.EasingSpawn);
        }

        private void StopSpawnAnimation()
        {
            if (_spawnTween != null)
            {
                _spawnTween.Kill();
                _spawnTween = null;
            }
        }

        #endregion
    }
}
