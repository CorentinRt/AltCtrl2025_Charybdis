using System;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class WindDetector : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioLoudnessDetection _detector;

        [SerializeField] private float _loudnessSensibility = 100f;
        [SerializeField] private float _loudnessTreshold = 0.1f;

        [SerializeField] private float _updateTime = 0.1f;

        private float _currentUpdateTime = 0f;

        public event Action<float> OnWindDetection;
        // ----- FIELDS ----- //

        private void Update()
        {
            _currentUpdateTime += Time.deltaTime;

            if (_currentUpdateTime > _updateTime)
            {
                float loudness = _detector.GetLoudnessFromMicrophone() * _loudnessSensibility;

                if (loudness < _loudnessTreshold) loudness = 0;

                Debug.Log($"Loudness : {loudness}");

                OnWindDetection?.Invoke(loudness);

                _currentUpdateTime = 0f;
            }
        }
    }
}
