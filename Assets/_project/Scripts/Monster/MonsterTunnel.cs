using UnityEngine.Splines;
using UnityEngine;
using System.Collections;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Collider2D))]
    public class MonsterTunnel : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Spline Settings")]
        [SerializeField] private SplineContainer _spline; 
        [SerializeField] private float _timeToTravel = 5f;
        [SerializeField] private bool _reverse = false;

        private float _splineLength;
        private float _timeTravelled = 0f;

        private bool _isMoving = false;

        [Header("References")]
        [SerializeField] private MonsterTunnel _otherTunnel;
        [SerializeField] private Collider2D _collider;

        private Monster _monsterInTunnel;
        // ----- FIELDS ----- //

        private void Start()
        {
            if (_spline == null)
            {
                Debug.LogError("Aucune spline assignée");
                return;
            }

            _splineLength = SplineUtility.CalculateLength(_spline.Spline, _spline.transform.localToWorldMatrix);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Monster"))
            {
                Debug.Log("monster in trigger");
                Monster monster = collision.GetComponent<Monster>();
                if (monster.IsTeleporting() || !monster.CanTeleport()) return;

                Teleport(monster);
            }
        }

        private void Teleport(Monster monster)
        {
            Debug.Log("Teleporting monster");
            _monsterInTunnel = monster;
            _monsterInTunnel.SetTeleporting(true);
            _timeTravelled = 0f;
            _isMoving = true;

            _monsterInTunnel.SetUnderground(true);

            // ----- AUDIO ----- //
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("Monster_TP");
            // ----- AUDIO ----- //

            // Collisions
            _otherTunnel.SetCollisions(false);
        }

        private void EndTeleport()
        {
            _monsterInTunnel.SetUnderground(false);
            _monsterInTunnel.SetTeleporting(false);
            _monsterInTunnel.SetCanMove(true);
            _monsterInTunnel.transform.position = _otherTunnel.transform.position;
        }

        private void Update()
        {
            if (_spline == null) return;

            if (_isMoving)
            {
                _timeTravelled += Time.deltaTime;

                float normalizedT = Mathf.Clamp01(_timeTravelled / _timeToTravel);

                if (_reverse)
                    normalizedT = 1 - normalizedT;

                // End
                if ((normalizedT >= 1f && !_reverse) || (normalizedT <= 0f && _reverse))
                {
                    _timeTravelled = _timeToTravel;
                    _isMoving = false;
                    EndTeleport();
                }

                // Set position on spline
                Vector3 worldPos = _spline.EvaluatePosition(normalizedT);
                _monsterInTunnel.transform.position = worldPos;
            }
        }

        private void SetCollisions(bool collision)
        {
            Debug.Log($"{gameObject.name}: collisions {collision}");
            _collider.enabled = collision;

            if (!collision)
                StartCoroutine(WaitAndSetCollisions(true));
        }

        private IEnumerator WaitAndSetCollisions(bool collision)
        {
            yield return new WaitForSeconds(_timeToTravel + 2f);
            SetCollisions(collision);
        }
    }
}
