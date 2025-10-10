using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Collider2D))]
    public class MonsterTyphoon : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("References")]
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _visuals;

        [Header("Values")]
        [SerializeField] private SO_MonsterTyphoonData _data;
        // ----- FIELDS ----- //

        private void Start()
        {
            DeactivateTyphon();
        }

        public void ActivateTyphon()
        {
            StartCoroutine(WaitAndActivateCollider());
            StartCoroutine(WaitAndDeactivateTyphon());

            _visuals.gameObject.SetActive(true);
            _animator.SetBool("Thyphon_Actif", true);
        }

        private IEnumerator WaitAndActivateCollider()
        {
            yield return new WaitForSeconds(_data.ActivationAnimTime);
            _collider.enabled = true;
        }

        private IEnumerator WaitAndDeactivateTyphon()
        {
            yield return new WaitForSeconds(_data.TimeBeforeDeactivate);
            _animator.SetBool("Thyphon_Actif", false);
            _collider.enabled = false;
            yield return new WaitForSeconds(_data.DeactivationAnimTime);
            DeactivateTyphon();
        }

        public void DeactivateTyphon()
        {
            _collider.enabled = false;
            _visuals.SetActive(false);
        }

        #region Trigger Enter / Exit
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null || collision.gameObject == null)
                return;

            if (collision.gameObject.CompareTag("Ship"))
            {
                collision.gameObject.GetComponent<IShipBehaviour>().GetShip().SetAffectedByTyphon(true, transform.position);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == null || collision.gameObject == null)
                return;

            if (collision.gameObject.CompareTag("Ship"))
            {
                collision.gameObject.GetComponent<IShipBehaviour>().GetShip().SetAffectedByTyphon(false, transform.position);
            }
        }
        #endregion
    }
}
