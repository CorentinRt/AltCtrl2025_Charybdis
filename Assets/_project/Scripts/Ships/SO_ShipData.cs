using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_ShipData", menuName = "ScriptableObjects/Datas/Ships/SO_ShipData", order = 1)]
    public class SO_ShipData : ScriptableObject
    {
        #region Fields
        [Header("Controlled")]

        [Header("Movements")]
        [SerializeField] private float _acceleration;
        [SerializeField] private float _accelerationSmooth;
        [SerializeField] private float _minSpeed;
        [SerializeField] private float _maxSpeed;

        [Header("Rotate")]
        [SerializeField] private float _rotateAcceleration;
        [SerializeField] private float _maxRotateSpeed;
        [SerializeField] private float _gouvernailDecelerationForce;

        [Space]

        [Header("Auto")]

        [Header("Movements")]
        [SerializeField] private float _minAutoSpeed;
        [SerializeField] private float _maxAutoSpeed;

        [Header("Rotate")]
        [SerializeField] private float _minAutoRotateSpeed;
        [SerializeField] private float _maxAutoRotateSpeed;
        
        [Header("Gouvernail")]
        [SerializeField] private int _gouvernailMaxAmplitude = 20;

        [Header("Other")]

        [Space]

        [Header("Destroy")]
        [SerializeField] private float _destroyDelay;

        [Header("Wind")]
        [SerializeField] private float _windForceMultiplier;

        [Header("Storm")]
        [SerializeField] private float _minCooldownBeforeChangeStormModifier;
        [SerializeField] private float _maxCooldownBeforeChangeStormModifier;

        [SerializeField] private float _minStormModifier;
        [SerializeField] private float _maxStormModifier;

        [Header("Typhon")]
        [SerializeField] private float _typhonAttractionForce = 70f;

        #endregion

        #region Properties
        public float Acceleration => _acceleration;
        public float AccelerationSmooth => _accelerationSmooth;
        public float MinSpeed => _minSpeed;
        public float MaxSpeed => _maxSpeed;

        public float RotateAcceleration => _rotateAcceleration;
        public float MaxRotateSpeed => _maxRotateSpeed;
        public float GouvernailDecelerationForce => _gouvernailDecelerationForce;

        public float MinAutoSpeed => _minAutoSpeed;
        public float MaxAutoSpeed => _maxAutoSpeed;
        public float MinAutoRotateSpeed => _minAutoRotateSpeed;
        public float MaxAutoRotateSpeed => _maxAutoRotateSpeed;

        public float DestroyDelay => _destroyDelay;

        public float MinCooldownBeforeChangeStormModifier => _minCooldownBeforeChangeStormModifier;
        public float MaxCooldownBeforeChangeStormModifier => _maxCooldownBeforeChangeStormModifier;

        public float MinStormModifier => _minStormModifier;
        public float MaxStormModifier => _maxStormModifier;

        public float TyphonAttractionForce => _typhonAttractionForce;

        public int GouvernailMaxAmplitude => _gouvernailMaxAmplitude;

        public float WindForceMultiplier => _windForceMultiplier;
        #endregion

    }
}
