using CREMOT.GameplayUtilities;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AltCtrl.Charybdis
{
    public class WindIndicator : GenericSingleton<WindIndicator>
    {
        // ----- FIELDS ----- //
        [Header("Datas")]
        [SerializeField] private SO_WindData _data;

        [Header("References")]
        [SerializeField] private GameObject _windVFX;

        [SerializeField] private Transform _directionVisual;

        private Coroutine _windEffectCoroutine;

        public event Action<Vector3> OnStartWindOnBoats; // Vector 3 = rotation indicator
        public event Action OnStopWindOnBoats;
        // ----- FIELDS ----- //

        private void Start()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnWindPressed += OnWindPressed;
            }

            _windVFX.SetActive(false);
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnWindPressed -= OnWindPressed;
            }
        }

        private void OnWindPressed(bool pressed)
        {
            if (!pressed || _windEffectCoroutine != null)
                return;

            StartWindEffect();
        }

        private void StartWindEffect()
        {
            if (_windEffectCoroutine != null)
                return;

            OnStartWindOnBoats?.Invoke(_directionVisual.up);

            // ----- AUDIO ----- //
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("Wind");
            // ----- AUDIO ----- //

            _windVFX.SetActive(true);

            _windEffectCoroutine = StartCoroutine(WindEffectCoroutine());
        }

        private void StopWindEffect()
        {
            OnStopWindOnBoats?.Invoke();

            // ----- AUDIO ----- //
            /*
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopLoopingSound("Wind");
                */
            // ----- AUDIO ----- //

            _windVFX.SetActive(false);
        }

        private void StopWindCoroutine()
        {
            if (_windEffectCoroutine != null)
            {
                StopCoroutine(_windEffectCoroutine);
                _windEffectCoroutine = null;
            }
        }

        private IEnumerator WindEffectCoroutine()
        {
            
            yield return new WaitForSeconds(_data.WindDuration);

            StopWindEffect();
            

            yield return new WaitForSeconds(_data.WindCooldown);

            StopWindCoroutine();

            yield return null;
        }
    }
}
