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
        [SerializeField] private float _minDelayBetweenToggles = 1f; // anti-spam (g mis haut pour les tests)
    
        private bool _inputState = false;  

        // Timers
        private float _transitionTimer = 0f;
        private bool _isTransitioning = false;
        private float _lastToggleTime;
        // ----- FIELDS ----- //

        private void Start()
        {
            _visuals.SetActive(false);
            _storm.Collider.enabled = false;
        }

        public void SetActive(bool active)
        {
            // Anti-spam
            if (Time.time - _lastToggleTime < _minDelayBetweenToggles && active)
                return;

            _lastToggleTime = Time.time;

            if (_inputState == active) // déjà bon état
                return;

            _inputState = active;
            _transitionTimer = 0f;
            _isTransitioning = true; // transi début ou fin

            // Animations & visuals
            if (active)
            {
                //Debug.Log($"Totem Activation demandée : {gameObject.name}");
                _visuals.SetActive(true);
                _animator.SetBool("Vortex_Actif", true);

                // ----- AUDIO ----- //
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound("Totem_Click");
                    AudioManager.Instance.PlaySound("Tempest", true);
                }
                // ----- AUDIO ----- //
            }
            else
            {
                //Debug.Log($"Totem Désactivation demandée : {gameObject.name}");
                _animator.SetBool("Vortex_Actif", false);

                // ----- AUDIO ----- //
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StopLoopingSound("Tempest");
                }
                // ----- AUDIO ----- //
            }
        }

        private void Update()
        {
            if (!_isTransitioning)
                return;

            _transitionTimer += Time.deltaTime;

            // Colliders & visuals
            if (_inputState) // Activation en cours
            {
                if (_transitionTimer >= _activationAnimTime)
                {
                    _storm.Collider.enabled = true;
                    _isTransitioning = false;
                }
            }
            else // Désactivation en cours
            {
                if (_transitionTimer >= _deactivationAnimTime)
                {
                    _storm.Collider.enabled = false;
                    _visuals.SetActive(false);
                    _isTransitioning = false;
                }
            }
        }
    }
}
