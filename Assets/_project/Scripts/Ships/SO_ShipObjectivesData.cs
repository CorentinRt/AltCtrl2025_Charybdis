using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_ShipObjectivesData", menuName = "ScriptableObjects/Datas/Ships/SO_ShipObjectivesData", order = 1)]
    public class SO_ShipObjectivesData : ScriptableObject
    {
        #region Fields
        [Header("Spawn Objective Parameters")]
        [SerializeField] private float _radiusCheckSpawnObjective = 1f;
        [SerializeField] private float _offsetFromBordersSpawnObjective = 1f;
        [SerializeField] private float _checkpointScaleMultiplier = 1f;
        #endregion

        #region Properties
        public float RadiusCheckSpawnObjective => _radiusCheckSpawnObjective;
        public float OffsetFromBordersSpawnObjective => _offsetFromBordersSpawnObjective;
        public float CheckpointScaleMultiplier => _checkpointScaleMultiplier;
        #endregion
    }
}
