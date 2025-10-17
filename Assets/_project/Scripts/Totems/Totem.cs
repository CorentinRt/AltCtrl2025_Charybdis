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

        private Coroutine _coroutineBuffer;
        // ----- FIELDS ----- //

        private void Start()
        {
            _visuals.SetActive(false);
            _storm.Collider.enabled = false;
        }

        public void ActivateTotem()
        {
            StopCurrentStormCoroutine();

            _coroutineBuffer = StartCoroutine(WaitAndActivateCollider());

            _visuals.gameObject.SetActive(true);
            _animator.SetBool("Vortex_Actif", true);

            // ----- AUDIO ----- //
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("Totem_Click");
                AudioManager.Instance.PlaySound("Tempest", true);
            }
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

            // ----- AUDIO ----- //
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopLoopingSound("Tempest");
            }
            // ----- AUDIO ----- //

            _storm.Collider.enabled = false;
            yield return new WaitForSeconds(_deactivationAnimTime);
            _visuals.SetActive(false);
        }

        private void StopCurrentStormCoroutine()
        {
            if (_coroutineBuffer != null)
            {
                StopCoroutine(_coroutineBuffer);
                _coroutineBuffer = null;
            }
        }

        public void DeactivateTotem()
        {
            StopCurrentStormCoroutine();

            _coroutineBuffer = StartCoroutine(WaitAndDeactivateTotem());
        }

        public void SetActive(bool active)
        {
            StopCurrentStormCoroutine();

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
