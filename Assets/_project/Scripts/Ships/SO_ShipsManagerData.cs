using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_ShipsManagerData", menuName = "ScriptableObjects/Datas/Ships/SO_ShipsManagerData", order = 1)]
    public class SO_ShipsManagerData : ScriptableObject
    {
        #region Fields
        [Header("Spawns")]
        [SerializeField] private float _spawnRandomMinRate;
        [SerializeField] private float _spawnRandomMaxRate;

        #endregion

        #region Properties
        public float SpawnRandomMinRate => _spawnRandomMinRate;
        public float SpawnRandomMaxRate => _spawnRandomMaxRate;

        #endregion

    }
}
