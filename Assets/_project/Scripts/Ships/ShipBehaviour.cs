using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace AltCtrl.Charybdis
{
    public class ShipBehaviour : MonoBehaviour, IShipBehaviour
    {
        #region Fields
        [Header("Init")]
        [SerializeField] private bool _autoInit;

        [Header("Components")]
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Transform _visualAnchor;

        [SerializeField] private GameObject _explosionIndicator;
        [SerializeField] private GameObject _validateIndicator;
        [SerializeField] private GameObject _stormIndicator;
        [SerializeField] private GameObject _inTyphonIndicator;

        [Space]

        [Header("Datas")]
        [SerializeField] private SO_ShipData _data;

        [Header("Auto")]
        [SerializeField] private bool _isAuto;
        [SerializeField] private GameObject _controlledIndicator;

        [Header("Trajectory")]
        [SerializeField] private LineRenderer _trajectoryLine;
        [SerializeField] private int _predictionSteps = 50;
        [SerializeField] private float _timeStep = 0.1f;

        [Header("Frequency")]
        [SerializeField] private TextMeshProUGUI _frequencyLabel;
        [SerializeField] private Transform _frequencyHolder;

        // Move
        private float _currentSpeed;

        // Rotate
        private float _currentAngularVelocity;

        // Move Rotate
        private Vector3[] _trajectoryPointsBuffer;

        private float _currentGouvernailInput;
        private float _turboValue;

        private float _autoGouvernailValue;
        private float _autoSpeed;

        // Wind
        private bool _affectedByWind;
        private Vector3 _windDirModifier;

        // Storm
        private bool _affectedByStorm;
        private float _currentStormModifier;
        private float _currentCooldownNewStormModifier;
        private float _cooldownNewStormModifier;

        // Destroy / Validate
        private Coroutine _destroyShipWithDelayCoroutine;
        private Coroutine _validateShipWithDelayCoroutine;
        private bool _isDestroyed;
        private bool _isValidated;

        // Frequency
        private (int, int) _associatedFrequency;
        private float _frequencyLabelOffset;

        // Bounds
        private Vector3 _screenBounds;
        private bool _isInsideBound;


        // typhon
        private bool _affectedByTyphon;
        private Vector3 _typhonCenter;

        // Gouvernail
        private float _currentGouvernailAmplitude;
        #endregion


        #region Properties
        public bool IsAuto => _isAuto;

        public (int, int) AssociatedFrequency => _associatedFrequency;

        #endregion

        public event Action<ShipBehaviour> OnShipDestroyed;
        public event Action<ShipBehaviour> OnShipValidated;


        private void Start()
        {
            _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

            if (_autoInit)
            {
                Init();
            }

            if (WindIndicator.Exist)
            {
                WindIndicator.Instance.OnStartWindOnBoats += OnStartWind;
                WindIndicator.Instance.OnStopWindOnBoats += OnStopWind;
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMoveTurboPressed += OnMoveTurboPressed;

                InputManager.Instance.OnMoveShipRotatorPressed += OnMoveShipRotateInput;
            }

            _frequencyLabelOffset = _frequencyLabel.transform.localPosition.y;

        }

        private void ResetValues()
        {
            _isAuto = true;
            _isInsideBound = false;
            _isDestroyed = false;
            _isValidated = false;
            _affectedByStorm = false;
            _affectedByTyphon = false;
            _affectedByWind = false;

            _currentStormModifier = 0f;
            _currentCooldownNewStormModifier = 0f;
            _cooldownNewStormModifier = 0f;

            _currentGouvernailAmplitude = 0f;

            _autoGouvernailValue = 0f;
            _autoSpeed = 0f;

            _currentGouvernailInput = 0f;
            _turboValue = 0f;

            _associatedFrequency = (-1, -1);

            _explosionIndicator.SetActive(false);
            _validateIndicator.SetActive(false);
            _stormIndicator.SetActive(false);
            _inTyphonIndicator.SetActive(false);
            _controlledIndicator.SetActive(false);

            _trajectoryLine.positionCount = 0;

            _trajectoryLine.gameObject.SetActive(true);

            _frequencyLabel.gameObject.SetActive(true);

            _rb.angularVelocity = 0f;
            _rb.linearVelocity = Vector2.zero;
        }

        public void Init()
        {
            ResetValues();
            
            _trajectoryPointsBuffer = new Vector3[_predictionSteps];

            if (RadioManager.Exist)
            {
                _associatedFrequency = RadioManager.Instance.GetNewAvailableFrequency();

                if (_associatedFrequency == (-1, -1))
                {
                    gameObject.SetActive(false);
                    ReactOnDestroyShip();
                    return;
                }

                RadioManager.Instance.AddFrequencyUsed(_associatedFrequency);

                _frequencyLabel.text = $"{_associatedFrequency.Item1} : {_associatedFrequency.Item2}";
            }

            if (ShipsManager.Exist)
            {
                ShipsManager.Instance.AddShip(this);
            }

            _currentGouvernailInput = UnityEngine.Random.Range(-_data.MinAutoRotateSpeed, _data.MaxAutoRotateSpeed);

            _autoSpeed = UnityEngine.Random.Range(_data.MinAutoSpeed, _data.MaxAutoSpeed);

            _rb.linearVelocity = transform.up * _autoSpeed;
        }

        private void OnDestroy()
        {
            if (WindIndicator.Exist)
            {
                WindIndicator.Instance.OnStartWindOnBoats -= OnStartWind;
                WindIndicator.Instance.OnStopWindOnBoats -= OnStopWind;
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMoveTurboPressed -= OnMoveTurboPressed;

                InputManager.Instance.OnMoveShipRotatorPressed -= OnMoveShipRotateInput;
            }
        }

        private void ReactOnDestroyShip()
        {
            if (RadioManager.Exist)
            {
                if (_associatedFrequency != (-1, -1))
                {
                    RadioManager.Instance.RemoveFrequencyUsed(_associatedFrequency);
                }
            }
        }

        private void OnMoveShipRotateInput(int dir)
        {
            if (!_isAuto)
                return;

            _currentGouvernailAmplitude += dir;

            _currentGouvernailAmplitude = Mathf.Clamp(_currentGouvernailAmplitude, -_data.GouvernailMaxAmplitude, _data.GouvernailMaxAmplitude);
        }

        private void Update()
        {
            CheckValidateShip();

            _frequencyHolder.transform.rotation = Quaternion.identity;

            if (_frequencyLabel.transform.localPosition.y > 0 && transform.position.y > Camera.main.transform.position.y)
            {
                _frequencyLabel.transform.localPosition = new Vector3(0f, -_frequencyLabelOffset, 0f);
            }
            else if (_frequencyLabel.transform.localPosition.y < 0 && transform.position.y < Camera.main.transform.position.y)
            {
                _frequencyLabel.transform.localPosition = new Vector3(0f, _frequencyLabelOffset, 0f);
            }

            UpdateStormEffect();
        }

        private void FixedUpdate()
        {
            if (_isDestroyed)
                return;

            if (_isAuto)
            {
                MoveAutoReworked();
                RotateAutoReworked();
            }
            else
            {
                MoveControlledReworked();
                RotateControlledReworked();
            }

            if (_affectedByTyphon)
            {
                FixedUpdateTyphonEffect();
            }
        }

        #region Turbo
        private void OnMoveTurboPressed(Vector2 value)
        {
            _turboValue = value.y;
        }
        #endregion

        #region Set Controlled / Auto
        [Button]
        private void DebugSetControlled()
        {
            SetControlledValue(true);
        }

        [Button]
        private void DebugSetAuto()
        {
            SetControlledValue(false);
        }

        public void SetControlledValue(bool controlled)
        {
            _isAuto = !controlled;

            _controlledIndicator.SetActive(controlled);
        }
        #endregion

        #region Controlled movements
        private void MoveControlledReworked()
        {
            float targetSpeed = _currentSpeed + _turboValue * Time.fixedDeltaTime * _data.Acceleration;

            targetSpeed = Mathf.Clamp(targetSpeed, _data.MinSpeed, _data.MaxSpeed);

            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.fixedDeltaTime * _data.AccelerationSmooth);

            _currentSpeed = Mathf.Clamp(targetSpeed, _data.MinSpeed, _data.MaxSpeed);

            Vector2 targetVelocity = _currentSpeed * transform.up;

            if (_affectedByWind)
            {
                targetVelocity += new Vector2(_windDirModifier.x, _windDirModifier.y) * _data.WindForceMultiplier;
            }

            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * _data.Acceleration);
        }

        private void RotateControlledReworked()
        {
            _currentGouvernailAmplitude += Input.GetAxis("Horizontal") * Time.fixedDeltaTime * _data.RotateAcceleration;

            float gourvernailPercent = Mathf.Abs(_currentGouvernailAmplitude) / _data.GouvernailMaxAmplitude;

            float targetAngularVelocity = _data.MaxRotateSpeed * gourvernailPercent * Mathf.Sign(_currentGouvernailAmplitude);

            targetAngularVelocity = Mathf.Lerp(_currentAngularVelocity, targetAngularVelocity, Time.fixedDeltaTime * _data.RotateAcceleration);

            if (_affectedByStorm)
            {
                targetAngularVelocity += _currentStormModifier * Time.fixedDeltaTime;
            }

            _currentAngularVelocity = targetAngularVelocity;

            if (!_affectedByStorm)
            {
                _currentAngularVelocity = Mathf.Clamp(_currentAngularVelocity, -_data.MaxRotateSpeed, _data.MaxRotateSpeed);
            }

            _rb.angularVelocity = _currentAngularVelocity;
        }

        #endregion

        #region Auto movements
        private void MoveAutoReworked()
        {
            float targetSpeed = _autoSpeed;

            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.fixedDeltaTime * _data.Acceleration);

            _currentSpeed = Mathf.Clamp(targetSpeed, _data.MinSpeed, _data.MaxSpeed);

            Vector2 targetVelocity = _currentSpeed * transform.up;

            if (_affectedByWind)
            {
                targetVelocity += new Vector2(_windDirModifier.x, _windDirModifier.y) * _data.WindForceMultiplier;
            }

            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * _data.Acceleration);
        }

        private void RotateAutoReworked()
        {
            _currentGouvernailAmplitude = Mathf.Lerp(_currentGouvernailAmplitude, 0f, Time.fixedDeltaTime * _data.GouvernailDecelerationForce);

            float gourvernailPercent = Mathf.Abs(_currentGouvernailAmplitude) / _data.GouvernailMaxAmplitude;

            float targetAngularVelocity = _data.MaxRotateSpeed * gourvernailPercent * Mathf.Sign(_currentGouvernailAmplitude);

            targetAngularVelocity = Mathf.Lerp(_currentAngularVelocity, targetAngularVelocity, Time.fixedDeltaTime * _data.RotateAcceleration);

            if (_affectedByStorm)
            {
                targetAngularVelocity += _currentStormModifier * Time.fixedDeltaTime;
            }

            _currentAngularVelocity = targetAngularVelocity;

            if (!_affectedByStorm)
            {
                _currentAngularVelocity = Mathf.Clamp(_currentAngularVelocity, -_data.MaxRotateSpeed, _data.MaxRotateSpeed);
            }

            _rb.angularVelocity = _currentAngularVelocity;
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
        public void DestroyShip()
        {
            if (_isDestroyed || _isValidated)
            {
                return;
            }

            _isDestroyed = true;

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            _visualAnchor.DOLocalRotate(new Vector3(0f, 0f, 720f), _data.DestroyDelay * 0.9f, RotateMode.FastBeyond360);
            
            _explosionIndicator.SetActive(true);
            _frequencyLabel.gameObject.SetActive(false);
            _trajectoryLine.gameObject.SetActive(false);

            if (_destroyShipWithDelayCoroutine == null)
            {
                OnShipDestroyed?.Invoke(this);

                if (ShipsManager.Exist)
                {
                    ShipsManager.Instance.RemoveShip(this);
                }

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

            gameObject.SetActive(false);    // Set active false instead of destroy for pool manager
            ReactOnDestroyShip();
        }

        private IEnumerator DestroyShipWithDelayCoroutine()
        {
            yield return new WaitForSeconds(_data.DestroyDelay);

            EndDestroyShipWithDelay();

            yield return null;
        }

        #endregion

        #region Validate Ship
        [Button]
        private void CheckValidateShip()
        {
            if (_isValidated || _isDestroyed)
                return;

            if (transform.position.x > -_screenBounds.x && transform.position.x < _screenBounds.x && transform.position.y > -_screenBounds.y && transform.position.y < _screenBounds.y)
            {
                if (!_isInsideBound)
                {
                    _isInsideBound = true;
                }
            }
            else
            {
                if (_isInsideBound)
                {
                    ValidateShip();
                }
            }
        }

        public void ValidateShip()
        {
            if (_isDestroyed || _isValidated)
                return;

            _isValidated = true;

            _validateIndicator.SetActive(true);

            if (_validateShipWithDelayCoroutine == null)
            {
                OnShipValidated?.Invoke(this);

                if (ShipsManager.Exist)
                {
                    ShipsManager.Instance.RemoveShip(this);
                }

                _validateShipWithDelayCoroutine = StartCoroutine(ValidateShipWithDelayCoroutine());
            }

        }

        private void EndValidateShipWithDelay()
        {
            if (_validateShipWithDelayCoroutine != null)
            {
                StopCoroutine(_validateShipWithDelayCoroutine);
                _validateShipWithDelayCoroutine = null;
            }

            gameObject.SetActive(false);    // Set active false instead of destroy for pool manager
            ReactOnDestroyShip();
        }

        private IEnumerator ValidateShipWithDelayCoroutine()
        {
            yield return new WaitForSeconds(_data.DestroyDelay);

            EndValidateShipWithDelay();

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

        #region Storm
        public void SetAffectedByStorm(bool affected)
        {
            _affectedByStorm = affected;

            _stormIndicator.SetActive(affected);

            if (_affectedByStorm)
            {
                SetNewRandomStormModifier();
            }
        }

        private void SetNewRandomStormModifier()
        {
            _currentCooldownNewStormModifier = 0f;

            _cooldownNewStormModifier = UnityEngine.Random.Range(_data.MinCooldownBeforeChangeStormModifier, _data.MaxCooldownBeforeChangeStormModifier);

            _currentStormModifier = UnityEngine.Random.Range(_data.MinStormModifier, _data.MaxStormModifier) * Mathf.Sign(-_currentStormModifier);

            _currentGouvernailInput = -_currentGouvernailInput;
        }

        private void UpdateStormEffect()
        {
            if (!_affectedByStorm)
                return;

            _currentCooldownNewStormModifier += Time.deltaTime;

            if (_currentCooldownNewStormModifier >= _cooldownNewStormModifier)
            {
                SetNewRandomStormModifier();
            }
        }

        #endregion

        #region Typhon
        public void SetAffectedByTyphon(bool affected, Vector3 typhonPos)
        {
            _affectedByTyphon = affected;

            _inTyphonIndicator.SetActive(affected);

            _typhonCenter = typhonPos;

            if (!affected)
            {
                _currentGouvernailInput = 0f;
            }
        }

        private void FixedUpdateTyphonEffect()
        {
            if (!_affectedByTyphon || _isValidated || _isDestroyed)
                return;

            Vector2 dirToCenter = (_typhonCenter - transform.position);
            float distance = dirToCenter.magnitude;

            if (distance < 0.5f)
            {
                DestroyShip();
                return;
            }

            dirToCenter.Normalize();

            float cross = transform.up.x * dirToCenter.y - transform.up.y * dirToCenter.x;

            _currentGouvernailInput += Time.fixedDeltaTime * -cross * _data.TyphonAttractionForce;
        }



        #endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision == null || collision.gameObject == null)
                return;

            if (collision.gameObject.CompareTag("Island") || collision.gameObject.CompareTag("Ship"))
            {
                if (_isDestroyed)
                    return;

                DestroyShip();
            }
        }

        #region IShipBehaviour
        public ShipBehaviour GetShip()
        {
            return this;
        }

        #endregion
    }
}
