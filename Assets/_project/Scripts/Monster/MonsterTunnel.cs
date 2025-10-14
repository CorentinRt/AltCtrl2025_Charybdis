using System.Collections;
using UnityEngine.Splines;
using UnityEngine;
using Codice.Client.BaseCommands.FastExport;
using System.Threading;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Collider2D))]
    public class MonsterTunnel : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Spline Settings")]
        [SerializeField] private SplineContainer _spline; 
        [SerializeField] private float _speed = 5f;
        [SerializeField] private bool _reverse = false;

        private float _splineLength;
        private float _distanceTravelled = 0f;

        private bool _isMoving = false;

        [Header("References")]
        [SerializeField] private MonsterTunnel _otherTunnel;

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

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Monster"))
            {
                Monster monster = collision.GetComponent<Monster>();
                if (monster.IsTeleporting() && monster.gameObject.activeSelf && _isMoving)
                {
                    monster.SetTeleporting(false);
                }
            }
        }

        private void Teleport(Monster monster)
        {
            Debug.Log("Teleporting monster");
            _monsterInTunnel = monster;
            _monsterInTunnel.SetTeleporting(true);
            _isMoving = true;

            // Ombre

            // ----- AUDIO ----- //
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("Monster_TP");
            // ----- AUDIO ----- //
        }

        private void EndTeleport()
        {
            // Pas ombre
            _monsterInTunnel.SetCanMove(true);
            _monsterInTunnel.transform.position = _otherTunnel.transform.position;
        }

        private void Update()
        {
            if (_spline == null) return;

            if (_isMoving)
            {
                _distanceTravelled += _speed * Time.deltaTime;

                // End
                if (_distanceTravelled > _splineLength)
                {
                    _distanceTravelled = _splineLength;
                    _isMoving = false;
                    EndTeleport();
                }

                // Set position on spline
                float normalizedT = Mathf.Clamp01(_distanceTravelled / _splineLength);
                Vector3 worldPos = _spline.EvaluatePosition(normalizedT);
                _monsterInTunnel.transform.position = worldPos;
            }
        }
    }
}
