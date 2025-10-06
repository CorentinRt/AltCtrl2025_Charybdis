using NaughtyAttributes;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class WindIndicator : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _rotatingSpeed = 5f;
        [SerializeField] private float _minLoudness = 3f;

        [Header("References")]
        [SerializeField] private WindDetector _detector;

        private bool _isBlowindLoud = false;
        // ----- FIELDS ----- //

        private void Start()
        {
            _detector.OnWindDetection += OnWindDetection;
        }

        private void OnWindDetection(float loudness)
        {
            if (loudness < _minLoudness)
            {
                _isBlowindLoud = false;
            }
            else
            {
                _isBlowindLoud = true;
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
