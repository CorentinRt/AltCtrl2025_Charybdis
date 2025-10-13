using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class Totem : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("References")]
        [SerializeField] private GameObject _visuals;
        [SerializeField] private Storm _storm;
        [SerializeField] private Animator _animator;

        [Header("Values")]
        [SerializeField] private float _activationAnimTime = 2f;
        [SerializeField] private float _deactivationAnimTime = 3f;
        // ----- FIELDS ----- //

        private void Start()
        {
            _visuals.SetActive(false);
            _storm.Collider.enabled = false;
        }

        public void ActivateTotem()
        {
            StartCoroutine(WaitAndActivateCollider());

            _visuals.gameObject.SetActive(true);
            _animator.SetBool("Vortex_Actif", true);

            // ----- AUDIO ----- //
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("Totem_Click");
            // ----- AUDIO ----- //
        }

        private IEnumerator WaitAndActivateCollider()
        {
            yield return new WaitForSeconds(_activationAnimTime);
            _storm.Collider.enabled = true;
        }

        private IEnumerator WaitAndDeactivateTotem()
        {
            _animator.SetBool("Vortex_Actif", false);
            _storm.Collider.enabled = false;
            yield return new WaitForSeconds(_deactivationAnimTime);
            _visuals.SetActive(false);
            DeactivateTotem();
        }

        public void DeactivateTotem()
        {
            StartCoroutine(WaitAndDeactivateTotem());
        }

        public void SetActive(bool active)
        {
            StopAllCoroutines();

            if (active)
            {
                ActivateTotem();
            }
            else
            {
                DeactivateTotem();
            }
        }
    }
}
