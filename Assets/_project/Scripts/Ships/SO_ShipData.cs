using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_ShipData", menuName = "ScriptableObjects/Datas/Ships/SO_ShipData", order = 1)]
    public class SO_ShipData : ScriptableObject
    {
        #region Fields
        [Header("Movements")]
        [SerializeField] private float _acceleration;
        [SerializeField] private float _minSpeed;
        [SerializeField] private float _maxSpeed;

        [SerializeField] private float _rotateSpeed;
        [SerializeField] private float _maxRotateSpeed;
        [SerializeField] private float _decelerationForce;

        [Header("Auto")]
        [SerializeField] private float _minAutoSpeed;
        [SerializeField] private float _maxAutoSpeed;

        [SerializeField] private float _minAutoRotateSpeed;
        [SerializeField] private float _maxAutoRotateSpeed;

        [Header("Destroy")]
        [SerializeField] private float _destroyDelay;

        [Header("Storm")]
        [SerializeField] private float _minCooldownBeforeChangeStormModifier;
        [SerializeField] private float _maxCooldownBeforeChangeStormModifier;

        [SerializeField] private float _minStormModifier;
        [SerializeField] private float _maxStormModifier;

        [Header("Typhon")]
        [SerializeField] private float _typhonAttractionForce = 70f;

        [Header("Gouvernail")]
        [SerializeField] private int _gouvernailMaxAmplitude = 20;
        #endregion

        #region Properties
        public float Acceleration => _acceleration;
        public float MinSpeed => _minSpeed;
        public float MaxSpeed => _maxSpeed;

        public float RotateSpeed => _rotateSpeed;
        public float MaxRotateSpeed => _maxRotateSpeed;
        public float DecelerationForce => _decelerationForce;

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
        #endregion

    }
}
