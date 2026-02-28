using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_HumanTutoController_Data", menuName = "ScriptableObjects/Datas/Tuto/SO_HumanTutoController_Data", order = 1)]
    public class SO_HumanTutoController_Data : ScriptableObject
    {
        #region Fields
        [Header("Phases wait duration : frequency tuto")]
        [SerializeField] private float _timeBeforeSpawnFirstShip = 5f;
        [SerializeField] private float _timeBeforeFrequencyTutoAppear = 1f;
        [SerializeField] private float _timeBeforeFreezeTime = 2f;

        [Header("Phases wait duration : Dangers & Islands")]
        [SerializeField] private float _timeBeforeDangerTutoAndIslandsAppear = 1.5f;

        [Header("Phases wait duration : Demo Game")]
        [SerializeField] private float _timeBeforeDemoGameStart = 8f;

        #endregion


        #region Properties
        public float TimeBeforeSpawnFirstShip => _timeBeforeSpawnFirstShip;
        public float TimeBeforeFrequencyTutoAppear => _timeBeforeFrequencyTutoAppear;
        public float TimeBeforeFreezeTime => _timeBeforeFreezeTime;

        public float TimeBeforeDangerTutoAndIslandsAppear => _timeBeforeDangerTutoAndIslandsAppear;

        public float TimeBeforeDemoGameStart => _timeBeforeDemoGameStart;

        #endregion
    }
}
