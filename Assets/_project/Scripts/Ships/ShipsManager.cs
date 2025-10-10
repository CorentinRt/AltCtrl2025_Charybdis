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

        [Header("Spawn Parameters")]
        [SerializeField] private LayerMask _islandsLayerMask;
        [SerializeField] private float _spawnDistanceCheck = 10f;
        [SerializeField] private float _checkRadius = 5f;

        private List<ShipBehaviour> _ships = new List<ShipBehaviour>();

        private Dictionary<(int, int), ShipBehaviour> _frequencyToShip = new Dictionary<(int, int), ShipBehaviour>();

        private float _currentTimeSpawn;
        private float _randomTimeSpawn;

        private Vector2 _screenBounds;

        private ShipBehaviour _currentControlledShip;

        private bool _enabledShipsSpawn;

        #endregion

        #region Properties


        #endregion

        public event Action OnDestroyShip;
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
                ship.OnShipValidated -= ReactOnValidateShip;
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
            ship.OnShipValidated += ReactOnValidateShip;

            _frequencyToShip[ship.AssociatedFrequency] = ship;
        }

        public void RemoveShip(ShipBehaviour ship)
        {
            if (ship == null)
                return;

            _ships.Remove(ship);

            ship.OnShipDestroyed -= ReactOnDestroyShip;
            ship.OnShipValidated -= ReactOnValidateShip;

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
            if (!_enabledShipsSpawn)
                return;

            _currentTimeSpawn += Time.deltaTime;

            if (_currentTimeSpawn >= _randomTimeSpawn)
            {
                DefineNewRandomTimeSpawn();

                SpawnShip();
            }
        }

        public void SetEnableShipsSpawn(bool enabled)
        {
            _enabledShipsSpawn = enabled;
        }

        private void DefineNewRandomTimeSpawn()
        {
            _currentTimeSpawn = 0f;
            _randomTimeSpawn = UnityEngine.Random.Range(_data.SpawnRandomMinRate, _data.SpawnRandomMaxRate);
        }

        private void SpawnShip(int iteration = 0)
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


            if (iteration < 10)
            {
                Debug.Log($"Iteration {iteration}", this);
                RaycastHit2D hit = Physics2D.CircleCast(randomPos, _checkRadius, dir, _spawnDistanceCheck, _islandsLayerMask);
                //RaycastHit2D hit = Physics2D.Raycast(randomPos, dir, _spawnDistanceCheck, _islandsLayerMask);

                DrawCircleCast(randomPos, _checkRadius, dir, _spawnDistanceCheck, Color.red, 2f);

                if (hit.collider != null)
                {
                    Debug.Log($"Hit : {hit.collider.gameObject.name}", hit.collider.gameObject);

                    SpawnShip(++iteration);
                    return;
                }
            }

            if (PoolManager.Instance != null)
            {
                GameObject ship = PoolManager.Instance.ActivateShip(randomPos, rot, true);
                ship.GetComponent<IShipBehaviour>().Init();
            }
        }

        #region Debug Circle Cast
        public void DrawCircleCast(Vector2 origin, float radius, Vector2 direction, float distance, Color color, float duration = 0f, int segments = 24)
        {
            // Cercle de départ
            DrawCircle(origin, radius, color, duration, segments);

            // Cercle de fin (là où le cast s'arrête)
            Vector2 end = origin + direction.normalized * distance;
            DrawCircle(end, radius, color * 0.8f, duration, segments);

            // Lignes reliant les deux cercles
            Vector2 dir = direction.normalized;
            Vector2 perp = new Vector2(-dir.y, dir.x); // vecteur perpendiculaire

            Vector2 startEdge1 = origin + perp * radius;
            Vector2 startEdge2 = origin - perp * radius;
            Vector2 endEdge1 = end + perp * radius;
            Vector2 endEdge2 = end - perp * radius;

            Debug.DrawLine(startEdge1, endEdge1, color, duration);
            Debug.DrawLine(startEdge2, endEdge2, color, duration);
        }

        public void DrawCircle(Vector2 center, float radius, Color color, float duration = 0f, int segments = 24)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + Vector2.right * radius;
            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                Debug.DrawLine(prevPoint, newPoint, color, duration);
                prevPoint = newPoint;
            }
        }

        #endregion

        public void ValidateAllShip()
        {
            ShipBehaviour[] allShips = GameObject.FindObjectsByType<ShipBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (ShipBehaviour ship in allShips)
            {
                ship.ValidateShip();
            }
        }

        public void DestroyAllShips()
        {
            ShipBehaviour[] allShips = GameObject.FindObjectsByType<ShipBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (ShipBehaviour ship in allShips)
            {
                ship.DestroyShip();
            }
        }

        private void ReactOnDestroyShip(ShipBehaviour ship)
        {
            ship.OnShipDestroyed -= ReactOnDestroyShip;

            OnDestroyShip?.Invoke();
        }

        private void ReactOnValidateShip(ShipBehaviour ship)
        {
            ship.OnShipValidated -= ReactOnValidateShip;

            OnValidateShip?.Invoke();
        }

        public void ReactOnChangeFrequency((int, int) frequency)
        {
            if (_currentControlledShip != null)
            {
                _currentControlledShip.SetControlledValue(false);
            }

            if (!_frequencyToShip.ContainsKey(frequency) || _frequencyToShip[frequency] == null)
            {
                return;
            }

            _currentControlledShip = _frequencyToShip[frequency];

            _currentControlledShip.SetControlledValue(true);

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
