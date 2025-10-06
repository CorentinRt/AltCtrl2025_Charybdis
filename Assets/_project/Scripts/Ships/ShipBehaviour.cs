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

        [Header("Trajectory")]
        [SerializeField] private LineRenderer _trajectoryLine;
        [SerializeField] private int _predictionSteps = 50;
        [SerializeField] private float _timeStep = 0.1f;

        [Header("Auto")]
        [SerializeField] private bool _isAuto;

        private float _currentGouvernailInput;

        private float _autoGouvernailValue;

        private float _autoSpeed;
        #endregion


        #region Properties
        public bool IsAuto => _isAuto;

        #endregion


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _currentGouvernailInput = Random.Range(-_data.MaxAutoRotateSpeed, _data.MaxAutoRotateSpeed);

            _autoSpeed = Random.Range(_data.MinAutoSpeed, _data.MaxAutoSpeed);
        }

        private void FixedUpdate()
        {
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

        private void Update()
        {
            PredictTrajectory();
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
        private void PredictTrajectory()
        {
            Vector2 startPos = _rb.position;
            Vector2 velocity = _rb.linearVelocity;     // vitesse actuelle en m/s
            float angularVel = _rb.angularVelocity;    // deg/s

            Vector3[] points = new Vector3[_predictionSteps];

            // Inversion du signe pour correspondre à Unity 2D (horaire = positif)
            float rotRadPerStep = angularVel * Mathf.Deg2Rad * _timeStep;

            Vector2 pos = startPos;
            Vector2 dir = velocity.normalized; // direction initiale = direction réelle du mouvement

            for (int i = 0; i < _predictionSteps; i++)
            {
                pos += dir * velocity.magnitude * _timeStep; // avance dans la direction actuelle

                // fais tourner la direction autour de Z
                float cos = Mathf.Cos(rotRadPerStep);
                float sin = Mathf.Sin(rotRadPerStep);
                dir = new Vector2(
                    dir.x * cos - dir.y * sin,
                    dir.x * sin + dir.y * cos
                );

                points[i] = pos;
            }

            _trajectoryLine.positionCount = _predictionSteps;
            _trajectoryLine.SetPositions(points);
        }

        #endregion

    }
}
