using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_GamePhasesData", menuName = "ScriptableObjects/Datas/GamePhases", order = 1)]
    public class SO_GamePhasesData : ScriptableObject
    {
        #region Fields
        [Header("PreGame")]
        [SerializeField] private int _cooldownSecondsDuration;

        #endregion

        #region Properties

        public int CooldownSecondsDuration => _cooldownSecondsDuration;

        #endregion

    }
}
