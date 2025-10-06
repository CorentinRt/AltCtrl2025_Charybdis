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


        private float _currentGouvernailInput;

        #endregion


        #region Properties


        #endregion

        private void FixedUpdate()
        {
            Move();

            Rotate();
        }

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

    }
}
