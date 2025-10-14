using CREMOT.GameplayUtilities;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AltCtrl.Charybdis
{
    public class WindIndicator : GenericSingleton<WindIndicator>
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _rotatingSpeed = 5f;
        [SerializeField] private float _minLoudness = 3f;
        [SerializeField] private float _minRandomTime = 2f;
        [SerializeField] private float _maxRandomTime = 5f;

        private float _currentRandomTime = 0f;
        private float _currentWaitedTime = 0f;

        private float _targetAngle;     
        private bool _isWaiting = false;

        [Header("References")]
        [SerializeField] private WindDetector _detector;
        [SerializeField] private Transform[] _visualAnchors;

        [SerializeField] private GameObject _windVFX;

        private bool _isBlowingLoud = false;
        
        public event Action<Vector3> OnStartWindOnBoats; // Vector 3 = rotation indicator
        public event Action OnStopWindOnBoats;
        // ----- FIELDS ----- //

        private void Start()
        {
            SetNewTargetRotation();

            _detector.OnWindDetection += OnWindDetection;

            _windVFX.SetActive(false);
        }

        private void OnWindDetection(float loudness)
        {
            if (loudness < _minLoudness)
            {
                if (_isBlowingLoud)
                {
                    _isBlowingLoud = false;
                    OnStopWindOnBoats?.Invoke();

                    // ----- AUDIO ----- //
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.StopLoopingSound("Wind");
                    // ----- AUDIO ----- //

                    _windVFX.SetActive(false);
                }
            }
            else
            {
                if (!_isBlowingLoud)
                {
                    _isBlowingLoud = true;

                    Debug.Log($"Start blowing loud direction :{_visualAnchors[0].up}");
                    OnStartWindOnBoats?.Invoke(_visualAnchors[0].up);

                    // ----- AUDIO ----- //
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlaySound("Wind", true);
                    // ----- AUDIO ----- //

                    _windVFX.SetActive(true);
                }
            }
        }

        private void Update()
        {
            if (!_isBlowingLoud)
            {
                if (!_isWaiting)
                {
                    bool allReached = true;

                    foreach (Transform visual in _visualAnchors)
                    {
                        float currentZ = visual.localEulerAngles.z;
                        float newZ = Mathf.MoveTowardsAngle(currentZ, _targetAngle, _rotatingSpeed * Time.deltaTime);
                        visual.localEulerAngles = new Vector3(0f, 0f, newZ);

                        if (Mathf.Abs(Mathf.DeltaAngle(currentZ, _targetAngle)) > 0.5f)
                            allReached = false;
                    }

                    if (allReached)
                    {
                        _isWaiting = true;
                        _currentRandomTime = Random.Range(_minRandomTime, _maxRandomTime);
                        _currentWaitedTime = 0f;
                    }
                }
                else
                {
                    _currentWaitedTime += Time.deltaTime;
                    if (_currentWaitedTime >= _currentRandomTime)
                    {
                        _isWaiting = false;
                        SetNewTargetRotation();
                    }
                }
            }
        }

        private void SetNewTargetRotation()
        {
            _targetAngle = Random.Range(0f, 360f);
        }
    }
}
