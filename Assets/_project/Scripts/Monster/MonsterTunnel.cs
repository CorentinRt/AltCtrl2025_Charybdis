using System.Collections;
using UnityEngine.Splines;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Collider2D))]
    public class MonsterTunnel : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _timeBeforeTP = 3f;

        [Header("Spline Settings")]
        [SerializeField] private SplineContainer _spline;  // ta spline dans la scène
        [SerializeField] private float _speed = 5f;

        [Header("References")]
        [SerializeField] private MonsterTunnel _otherTunnel;

        private Monster _monsterInTunnel;
        // ----- FIELDS ----- //

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Monster"))
            {
                Monster monster = collision.GetComponent<Monster>();
                if (monster.IsTeleporting() || !monster.CanTeleport()) return;

                _monsterInTunnel = monster;
                _monsterInTunnel.SetTeleporting(true);
                _monsterInTunnel.SetVisibilityMonster(false);
                StartCoroutine(WaitAndActivateMonster());

                // ----- AUDIO ----- //
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySound("Monster_TP");
                // ----- AUDIO ----- //
            }
        }

        private IEnumerator WaitAndActivateMonster()
        {
            yield return new WaitForSeconds(_timeBeforeTP);
            _monsterInTunnel.SetVisibilityMonster(true);
            _monsterInTunnel.SetCanMove(true);
            _monsterInTunnel.transform.position = _otherTunnel.transform.position;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Monster"))
            {
                Monster monster = collision.GetComponent<Monster>();
                if (monster.IsTeleporting() && monster.gameObject.activeSelf)
                {
                    Debug.Log("monster exit null");
                    monster.SetTeleporting(false);
                }
            }
        }
    }
}
