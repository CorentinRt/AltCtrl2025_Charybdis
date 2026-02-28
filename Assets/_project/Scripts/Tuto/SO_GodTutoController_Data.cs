using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_GodTutoController_Data", menuName = "ScriptableObjects/Datas/Tuto/SO_GodTutoController_Data", order = 1)]
    public class SO_GodTutoController_Data : ScriptableObject
    {
        #region Fields
        [Header("Phases wait duration : Move & Destroy Ship")]
        [SerializeField] private float _timeBeforeMoveTutoAppear = 3f;
        [SerializeField] private float _timeBeforeSpawnFirstShip = 8f;

        [Header("Phases wait duration : Beer")]
        [SerializeField] private float _timeBeforeTutoBeerAppear = 1f;

        [Header("Phases wait duration : Demo Game")]
        [SerializeField] private float _timeBeforeSpawnIslands = 2f;
        [SerializeField] private float _timeBeforeSpawnShipsDemoGame = 2f;

        #endregion


        #region Properties
        public float TimeBeforeMoveTutoAppear => _timeBeforeMoveTutoAppear;
        public float TimeBeforeSpawnFirstShip => _timeBeforeSpawnFirstShip;
        public float TimeBeforeTutoBeerAppear => _timeBeforeTutoBeerAppear;

        public float TimeBeforeSpawnIslands => _timeBeforeSpawnIslands;

        public float TimeBeforeSpawnShipsDemoGame => _timeBeforeSpawnShipsDemoGame;

        #endregion
    }
}
