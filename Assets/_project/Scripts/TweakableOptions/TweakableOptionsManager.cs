using CREMOT.GameplayUtilities;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class TweakableOptionsManager : GenericSingleton<TweakableOptionsManager>
    {
        #region Fields
        [Header("Tweaking Game Options")]
        [SerializeField] private SO_ShipsManagerData _shipsManagerData;
        [SerializeField] private SO_ShipData _shipData;
        [SerializeField] private SO_ShipObjectivesData _shipObjectivesData;
        [SerializeField] private SO_MonsterData _monsterData;
        [SerializeField] private SO_VictoryData _victoryData;

        static public string _maxShipOptionKey = "TWEAKOPTION_MaxShips";
        static public string _scaleCheckpointsOptionKey = "TWEAKOPTION_ScaleCheckpoints";
        static public string _shipsMaxSpeedOptionKey = "TWEAKOPTION_ShipsMaxSpeed";
        static public string _shipsMinSpeedOptionKey = "TWEAKOPTION_ShipsMinSpeed";
        static public string _monsterMaxSpeedOptionKey = "TWEAKOPTION_MonsterMaxSpeed";
        static public string _maxTimeSpawnShipsOptionKey = "TWEAKOPTION_MaxTimeSpawnShips";
        static public string _minTimeSpawnShipsOptionKey = "TWEAKOPTION_MinTimeSpawnShips";
        static public string _godPointsFactorOptionKey = "TWEAKOPTION_GodPointsFactor";
        static public string _humansPointsFactorOptionKey = "TWEAKOPTION_HumansPointsFactor";

        #endregion

        #region Properties


        #endregion

        protected override void Awake()
        {
            base.Awake();

            transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }

        #region Set Tweaking Options
        public void SetMaxShips(float value)
        {
            PlayerPrefs.SetInt(_maxShipOptionKey, (int)value);
        }

        public void SetScaleCheckpoints(float value)
        {
            PlayerPrefs.SetFloat(_scaleCheckpointsOptionKey, value);
        }

        public void SetShipsMaxSpeed(float value)
        {
            PlayerPrefs.SetFloat(_shipsMaxSpeedOptionKey, value);
        }

        public void SetShipsMinSpeed(float value)
        {
            PlayerPrefs.SetFloat(_shipsMinSpeedOptionKey, value);
        }

        public void SetMonsterMaxSpeed(float value)
        {
            PlayerPrefs.SetFloat(_monsterMaxSpeedOptionKey, value);
        }

        public void SetMaxTimeSpawnShips(float value)
        {
            PlayerPrefs.SetFloat(_maxTimeSpawnShipsOptionKey, value);
        }

        public void SetMinTimeSpawnShips(float value)
        {
            PlayerPrefs.SetFloat(_minTimeSpawnShipsOptionKey, value);
        }

        public void SetGodPointsFactor(float value)
        {
            PlayerPrefs.SetFloat(_godPointsFactorOptionKey, value);
        }

        public void SetHumansPointsFactor(float value)
        {
            PlayerPrefs.SetFloat(_humansPointsFactorOptionKey, value);
        }

        #endregion

        #region Tweak Options Getters
        public int GetMaxShips()
        {
            return PlayerPrefs.GetInt(_maxShipOptionKey, _shipsManagerData.MaxShips);
        }

        public float GetScaleCheckpoints()
        {
            return PlayerPrefs.GetFloat(_scaleCheckpointsOptionKey, _shipObjectivesData.CheckpointScaleMultiplier);
        }

        public float GetShipsMaxSpeed()
        {
            return PlayerPrefs.GetFloat(_shipsMaxSpeedOptionKey, _shipData.MaxSpeed);
        }

        public float GetShipsMinSpeed()
        {
            return PlayerPrefs.GetFloat(_shipsMinSpeedOptionKey, _shipData.MinSpeed);
        }

        public float GetMonsterMaxSpeed()
        {
            return PlayerPrefs.GetFloat(_monsterMaxSpeedOptionKey, _monsterData.MoveSpeed);
        }

        public float GetMaxTimeSpawnShips()
        {
            return PlayerPrefs.GetFloat(_maxTimeSpawnShipsOptionKey, _shipsManagerData.SpawnRandomMaxRate);
        }

        public float GetMinTimeSpawnShips()
        {
            return PlayerPrefs.GetFloat(_minTimeSpawnShipsOptionKey, _shipsManagerData.SpawnRandomMinRate);
        }

        public float GetGodPointsFactor()
        {
            return PlayerPrefs.GetFloat(_godPointsFactorOptionKey, _victoryData.GodPersonalMultiplier);
        }

        public float GetHumansPointsFactor()
        {
            return PlayerPrefs.GetFloat(_humansPointsFactorOptionKey, _victoryData.HumanPersonalMultiplier);
        }


        #endregion

    }
}
