using System;
using UnityEngine;
using UnityEngine.UI;

namespace AltCtrl.Charybdis
{
    public class WindDetector : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _loudnessSensibility = 100f;
        [SerializeField] private float _loudnessTreshold = 0.1f;
        [SerializeField] private float _maxLoudness = 50f;

        [SerializeField] private float _updateTime = 0.1f;

        [Header("References")]
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioLoudnessDetection _detector;
        [SerializeField] private Slider _slider;

        private float _currentUpdateTime = 0f;

        public event Action<float> OnWindDetection;
        // ----- FIELDS ----- //

        private void Start()
        {
            if (_slider != null)
            {
                _slider.maxValue = _maxLoudness;
            }
        }

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

                UpdateLoudnessSlider(loudness);
            }
        }

        private void UpdateLoudnessSlider(float loudness)
        {
            if (_slider == null) return;

            _slider.value = loudness;
        }
    }
}
