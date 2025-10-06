using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_ShipData", menuName = "ScriptableObjects/Datas/Ships/SO_ShipData", order = 1)]
    public class SO_ShipData : ScriptableObject
    {
        #region Fields
        [Header("Movements")]
        [SerializeField] private float _acceleration;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _rotateSpeed;

        #endregion

        #region Properties
        public float Acceleration => _acceleration;
        public float MaxSpeed => _maxSpeed;
        public float RotateSpeed => _rotateSpeed;

        #endregion

    }
}
