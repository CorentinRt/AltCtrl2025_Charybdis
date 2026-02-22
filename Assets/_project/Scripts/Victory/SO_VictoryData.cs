using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_VictoryData", menuName = "ScriptableObjects/Datas/Victory/SO_VictoryData", order = 1)]
    public class SO_VictoryData : ScriptableObject
    {
        #region Fields

        [Header("Time multiplier")]
        [SerializeField] private float _add1MultiplierEachTime = 10f;

        [Header("Side Mulitpliers")]
        [SerializeField] private float _godPersonalMultiplier = 1f;
        [SerializeField] private float _humanPersonalMultiplier = 1f;

        #endregion

        #region Properties
        public float Add1MultiplierEachTime => _add1MultiplierEachTime;
        public float GodPersonalMultiplier => _godPersonalMultiplier;
        public float HumanPersonalMultiplier => _humanPersonalMultiplier;

        #endregion
    }
}
