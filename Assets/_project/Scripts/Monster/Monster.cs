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

        [Header("References")]
        [SerializeField] private MonsterTarget _target;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _monsterVisuals;

        private bool _isMoving = true;
        private bool _canTyphoon = true;

        private Rigidbody2D _rb;

        private bool _canMove = true;
        // ----- FIELDS ----- //

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
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

            Vector2 currentPosition = _rb.position;
            Vector2 targetPosition = _target.GetComponent<Rigidbody2D>().position; 

            Vector2 direction = targetPosition - currentPosition;
            float distance = direction.magnitude;

            // On target
            if (distance < 0.05f)
            {
                _rb.MovePosition(targetPosition);
                OnMonsterTyphoon(true);
                return;
            }

            Vector2 move = direction.normalized * _monsterData.MoveSpeed * deltaTime;
            Vector2 newPosition = currentPosition + move;

            _rb.MovePosition(newPosition);
        }

        private void OnMonsterTyphoon(bool pressed)
        {
            //Debug.Log("typhoon");

            if (!_canMove) return;

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

        #region Typhoon Indication
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
        #endregion

        private IEnumerator StartTyphoonCooldown()
        {
            //yield return new WaitForSeconds(_monsterData.TyphoonCooldown - _monsterData.IndicationBeforeTyphoon);
            //OnMonsterTyphoonIndication();
            //yield return new WaitForSeconds(_monsterData.IndicationBeforeTyphoon);

            yield return new WaitForSeconds(_monsterData.TyphoonCooldown);
            _canTyphoon = true;

            //OnMonsterTyphoon(true);
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
    }
}
