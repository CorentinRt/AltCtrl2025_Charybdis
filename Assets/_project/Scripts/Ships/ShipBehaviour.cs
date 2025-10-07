using DG.Tweening;
using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class ShipBehaviour : MonoBehaviour
    {
        #region Fields
        [Header("Components")]
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Transform _visualAnchor;

        [Space]

        [Header("Datas")]
        [SerializeField] private SO_ShipData _data;

        [Header("Auto")]
        [SerializeField] private bool _isAuto;

        [Header("Trajectory")]
        [SerializeField] private LineRenderer _trajectoryLine;
        [SerializeField] private int _predictionSteps = 50;
        [SerializeField] private float _timeStep = 0.1f;

        private Vector3[] _trajectoryPointsBuffer;

        private float _currentGouvernailInput;

        private float _autoGouvernailValue;
        private float _autoSpeed;

        private bool _isDestroyed;

        private bool _affectedByWind;
        private Vector3 _windDirModifier;

        private Coroutine _destroyShipWithDelayCoroutine;
        #endregion


        #region Properties
        public bool IsAuto => _isAuto;

        #endregion

        public event Action<ShipBehaviour> OnShipDestroyed;


        private void Awake()
        {
            _trajectoryPointsBuffer = new Vector3[_predictionSteps];
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (ShipsManager.Exist)
            {
                ShipsManager.Instance.AddShip(this);
            }

            if (WindIndicator.Exist)
            {
                WindIndicator.Instance.OnStartWindOnBoats += OnStartWind;
                WindIndicator.Instance.OnStopWindOnBoats += OnStopWind;
            }

            _currentGouvernailInput = UnityEngine.Random.Range(-_data.MaxAutoRotateSpeed, _data.MaxAutoRotateSpeed);

            _autoSpeed = UnityEngine.Random.Range(_data.MinAutoSpeed, _data.MaxAutoSpeed);
        }

        private void OnDestroy()
        {
            if (WindIndicator.Exist)
            {
                WindIndicator.Instance.OnStartWindOnBoats -= OnStartWind;
                WindIndicator.Instance.OnStopWindOnBoats -= OnStopWind;
            }
        }

        private void FixedUpdate()
        {
            if (_isDestroyed)
                return;

            if (_isAuto)
            {
                MoveAuto();
                RotateAuto();
            }
            else
            {
                Move();
                Rotate();
            }
        }


        public void SetAutoGouvernailValue(float value)
        {
            _autoGouvernailValue = value;

        }

        #region Movements
        private void Move()
        {
            float deltaInputVertical = Input.GetAxis("Vertical");

            Vector3 tempVelocity = _rb.linearVelocity;

            tempVelocity += transform.up * Time.fixedDeltaTime * _data.Acceleration;


            if (tempVelocity.magnitude * deltaInputVertical < _data.MinSpeed)
            {
                tempVelocity = tempVelocity.normalized;

                tempVelocity *= _data.MinSpeed;
            }
            else
            {
                tempVelocity *= deltaInputVertical;
                tempVelocity = Vector3.ClampMagnitude(tempVelocity, _data.MaxSpeed);
            }

            _rb.linearVelocity = tempVelocity;
        }

        private void MoveAuto()
        {
            _rb.linearVelocity = transform.up * _autoSpeed;

            if (_affectedByWind)
            {
                _rb.linearVelocity += new Vector2(_windDirModifier.x, _windDirModifier.y);
            }
        }

        private void Rotate()
        {
            float deltaInputHorizontal = Input.GetAxis("Horizontal");

            if (deltaInputHorizontal != 0f)
            {
                _currentGouvernailInput += deltaInputHorizontal * Time.fixedDeltaTime * _data.RotateSpeed;
            }
            else
            {
                _currentGouvernailInput = Mathf.Lerp(_currentGouvernailInput, 0f, Time.fixedDeltaTime * _data.DecelerationForce);
            }

            _currentGouvernailInput = Mathf.Clamp(_currentGouvernailInput, -_data.MaxRotateSpeed, _data.MaxRotateSpeed);

            _rb.angularVelocity = -_currentGouvernailInput;
        }

        private void RotateAuto()
        {
            _currentGouvernailInput = Mathf.Clamp(_currentGouvernailInput, _data.MinAutoRotateSpeed, _data.MaxAutoRotateSpeed);

            _rb.angularVelocity = -_currentGouvernailInput;
        }

        #endregion

        #region Trajectory
        public void PredictTrajectory()
        {
            Vector2 startPos = _rb.position;
            Vector2 velocity = _rb.linearVelocity;
            float angularVel = _rb.angularVelocity;

            if (_trajectoryPointsBuffer == null || _trajectoryPointsBuffer.Length != _predictionSteps)
            {
                _trajectoryPointsBuffer = new Vector3[_predictionSteps];
            }

            float rotRadPerStep = angularVel * Mathf.Deg2Rad * _timeStep;

            Vector2 pos = startPos;
            Vector2 dir = velocity.normalized;

            for (int i = 0; i < _predictionSteps; i++)
            {
                pos += dir * velocity.magnitude * _timeStep;

                float cos = Mathf.Cos(rotRadPerStep);
                float sin = Mathf.Sin(rotRadPerStep);

                dir = new Vector2(
                    dir.x * cos - dir.y * sin,
                    dir.x * sin + dir.y * cos
                );

                _trajectoryPointsBuffer[i] = pos;
            }

            _trajectoryLine.positionCount = _predictionSteps;
            _trajectoryLine.SetPositions(_trajectoryPointsBuffer);
        }

        #endregion

        #region DestroyShip
        [Button]
        private void DestroyShip()
        {
            if (_isDestroyed)
            {
                return;
            }

            _isDestroyed = true;

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            _visualAnchor.DOLocalRotate(new Vector3(0f, 0f, 720f), _data.DestroyDelay * 0.9f, RotateMode.FastBeyond360);
            
            if (ShipsManager.Exist)
            {
                ShipsManager.Instance.RemoveShip(this);
            }

            if (_destroyShipWithDelayCoroutine == null)
            {
                OnShipDestroyed?.Invoke(this);

                _destroyShipWithDelayCoroutine = StartCoroutine(DestroyShipWithDelayCoroutine());
            }
        }

        private void EndDestroyShipWithDelay()
        {
            if (_destroyShipWithDelayCoroutine != null)
            {
                StopCoroutine(_destroyShipWithDelayCoroutine);
                _destroyShipWithDelayCoroutine = null;
            }

            Destroy(gameObject);
        }

        private IEnumerator DestroyShipWithDelayCoroutine()
        {
            yield return new WaitForSeconds(_data.DestroyDelay);

            EndDestroyShipWithDelay();

            yield return null;
        }

        #endregion

        #region Wind Reactions
        private void OnStartWind(Vector3 dir)
        {
            _affectedByWind = true;
            _windDirModifier = dir;
        }

        private void OnStopWind()
        {
            _affectedByWind = false;
            _windDirModifier = Vector3.zero;
        }
        #endregion

    }
}
