using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_RadioData", menuName = "ScriptableObjects/Datas/Radio/SO_RadioData", order = 1)]
    public class SO_RadioData : ScriptableObject
    {
        #region Fields
        [Header("Main Frequency")]
        [SerializeField] private int _mainFrequencyMin;
        [SerializeField] private int _mainFrequencyMax;

        [Header("External Frequency")]
        [SerializeField] private int _externalFrequencyMin;
        [SerializeField] private int _externalFrequencyMax;

        [Header("Switch frequency")]
        [SerializeField] private float _timeToHoldIfNoShipControlled;
        [SerializeField] private float _timeToHoldIfHasShipControlled;

        #endregion

        #region Properties
        public int MainFrequencyMin => _mainFrequencyMin;
        public int MainFrequencyMax => _mainFrequencyMax;

        public int ExternalFrequencyMin => _externalFrequencyMin;
        public int ExternalFrequencyMax => _externalFrequencyMax;

        public float TimeToHoldIfNoShipControlled => _timeToHoldIfNoShipControlled;
        public float TimeToHoldIfHasShipControlled => _timeToHoldIfHasShipControlled;

        #endregion

    }
}
