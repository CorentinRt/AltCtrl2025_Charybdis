using CREMOT.GameplayUtilities;
using System;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class WindIndicator : GenericSingleton<WindIndicator>
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _rotatingSpeed = 5f;
        [SerializeField] private float _minLoudness = 3f;

        [Header("References")]
        [SerializeField] private WindDetector _detector;

        private bool _isBlowindLoud = false;
        
        public event Action<Vector3> OnStartWindOnBoats; // Vector 3 = rotation indicator
        public event Action OnStopWindOnBoats;
        // ----- FIELDS ----- //

        private void Start()
        {
            _detector.OnWindDetection += OnWindDetection;
        }

        private void OnWindDetection(float loudness)
        {
            if (loudness < _minLoudness)
            {
                if (_isBlowindLoud)
                {
                    _isBlowindLoud = false;
                    OnStopWindOnBoats?.Invoke();
                }
            }
            else
            {
                if (!_isBlowindLoud)
                {
                    _isBlowindLoud = true;
                    Debug.Log($"Start blowing loud direction :{transform.rotation.eulerAngles}");
                    OnStartWindOnBoats?.Invoke(transform.rotation.eulerAngles);
                }
            }
        }

        private void Update()
        {
            if (!_isBlowindLoud)
            {
                transform.Rotate(Vector3.forward * _rotatingSpeed * Time.deltaTime);
            }
        }
    }
}
