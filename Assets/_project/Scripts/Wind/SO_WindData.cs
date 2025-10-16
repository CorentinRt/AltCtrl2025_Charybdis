using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_WindData", menuName = "ScriptableObjects/Datas/Wind/SO_WindData", order = 1)]
    public class SO_WindData : ScriptableObject
    {
        #region Fields
        [SerializeField] private float _windDuration;
        [SerializeField] private float _windCooldown;

        #endregion

        #region Properties
        public float WindDuration => _windDuration;
        public float WindCooldown => _windCooldown;

        #endregion

    }
}
