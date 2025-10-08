using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using CREMOT.GameplayUtilities;
using static UnityEngine.GraphicsBuffer;
using System;

namespace AltCtrl.Charybdis
{
    public class ShipsManager : GenericSingleton<ShipsManager>
    {
        private enum SPAWN_LOCATION
        {
            TOP = 0,
            LEFT = 1,
            RIGHT = 2,
            BOTTOM = 3
        }

        #region Fields
        [Header("Datas")]
        [SerializeField] private SO_ShipsManagerData _data;

        [Header("Prefab")]
        [SerializeField] private GameObject _shipPrefab;


        private List<ShipBehaviour> _ships = new List<ShipBehaviour>();

        private Dictionary<(int, int), ShipBehaviour> _frequencyToShip = new Dictionary<(int, int), ShipBehaviour>();

        private float _currentTimeSpawn;
        private float _randomTimeSpawn;

        private Vector2 _screenBounds;

        private ShipBehaviour _currentControlledShip;

        #endregion

        #region Properties


        #endregion

        // TO DO : S'inscrire ici pour détecter bateaux détruits pour la barre de victoire
        public event Action OnDestroyShip;

        // TO DO : S'inscrire ici pour détecter bateaux validé pour la barre de victoire
        public event Action OnValidateShip;

        private void Start()
        {
            _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

            if (RadioManager.Exist)
            {
                RadioManager.Instance.OnChangeFrequency += ReactOnChangeFrequency;
            }

            DefineNewRandomTimeSpawn();
        }

        private void OnDestroy()
        {
            foreach (ShipBehaviour ship in _ships)
            {
                ship.OnShipDestroyed -= ReactOnDestroyShip;
            }

            if (RadioManager.Exist)
            {
                RadioManager.Instance.OnChangeFrequency -= ReactOnChangeFrequency;
            }
        }

        public void AddShip(ShipBehaviour ship)
        {
            if (ship == null)
                return;

            _ships.Add(ship);

            ship.OnShipDestroyed += ReactOnDestroyShip;

            _frequencyToShip[ship.AssociatedFrequency] = ship;
        }

        public void RemoveShip(ShipBehaviour ship)
        {
            if (ship == null)
                return;

            _ships.Remove(ship);

            ship.OnShipDestroyed -= ReactOnDestroyShip;

            _frequencyToShip[ship.AssociatedFrequency] = null;
        }

        private void Update()
        {
            foreach (ShipBehaviour shipBehaviour in _ships)
            {
                if (shipBehaviour == null)
                    continue;

                shipBehaviour.PredictTrajectory();
            }

            HandleShipsSpawn();
        }

        private void HandleShipsSpawn()
        {
            _currentTimeSpawn += Time.deltaTime;

            if (_currentTimeSpawn >= _randomTimeSpawn)
            {
                DefineNewRandomTimeSpawn();

                SpawnShip();
            }
        }

        private void DefineNewRandomTimeSpawn()
        {
            _currentTimeSpawn = 0f;
            _randomTimeSpawn = UnityEngine.Random.Range(_data.SpawnRandomMinRate, _data.SpawnRandomMaxRate);
        }

        private void SpawnShip()
        {
            SPAWN_LOCATION randomSpawn = (SPAWN_LOCATION)UnityEngine.Random.Range(0, 4);

            Vector3 centerPos = Camera.main.transform.position;

            Vector3 targetPosition;

            targetPosition.x = UnityEngine.Random.Range(-_screenBounds.x, _screenBounds.x);
            targetPosition.y = UnityEngine.Random.Range(-_screenBounds.y, _screenBounds.y);

            Vector2 randomPos = Vector2.zero;

            switch (randomSpawn)
            {
                case SPAWN_LOCATION.TOP:
                    randomPos.x = targetPosition.x;
                    randomPos.y = _screenBounds.y + 1f;
                    break;

                case SPAWN_LOCATION.LEFT:
                    randomPos.x = -_screenBounds.x - 1f;
                    randomPos.y = targetPosition.y;
                    break;

                case SPAWN_LOCATION.RIGHT:
                    randomPos.x = _screenBounds.x + 1f;
                    randomPos.y = targetPosition.y;
                    break;

                case SPAWN_LOCATION.BOTTOM:
                    randomPos.x = targetPosition.x;
                    randomPos.y = -_screenBounds.y - 1f;
                    break;
            }

            Vector2 dir = -randomPos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            Instantiate(_shipPrefab, randomPos, rot);
        }

        private void ReactOnDestroyShip(ShipBehaviour ship)
        {
            ship.OnShipDestroyed -= ReactOnDestroyShip;

            OnDestroyShip?.Invoke();
        }

        private void ReactOnValidateShip(ShipBehaviour ship)
        {
            ship.OnShipValidated -= ReactOnDestroyShip;

            OnValidateShip?.Invoke();
        }

        private void ReactOnChangeFrequency((int, int) frequency)
        {
            if (_currentControlledShip != null)
            {
                _currentControlledShip.SetAutoValue(false);
            }

            if (!_frequencyToShip.ContainsKey(frequency) || _frequencyToShip[frequency] == null)
            {
                return;
            }

            _currentControlledShip = _frequencyToShip[frequency];

            _currentControlledShip.SetAutoValue(true);

        }

        public bool CheckShipWithFrequencyExist((int, int) frequency)
        {
            if (!_frequencyToShip.ContainsKey(frequency) || _frequencyToShip[frequency] == null)
            {
                return false;
            }

            return true;
        }
    }
}
