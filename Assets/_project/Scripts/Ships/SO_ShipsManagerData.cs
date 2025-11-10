using System.Collections.Generic;
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
        [SerializeField] private int _maxShips = 3;

        [Header("Customization")]
        [SerializeField] private List<Color> _objectivesColors = new List<Color>();
        [SerializeField] private List<Texture2D> _objectivesMotifs = new List<Texture2D>();


        #endregion

        #region Properties
        public float SpawnRandomMinRate => _spawnRandomMinRate;
        public float SpawnRandomMaxRate => _spawnRandomMaxRate;
        public int MaxShips => _maxShips;

        public List<Color> ObjectivesColors => _objectivesColors;

        public List<Texture2D> ObjectivesMotifs => _objectivesMotifs;

        #endregion

    }
}
