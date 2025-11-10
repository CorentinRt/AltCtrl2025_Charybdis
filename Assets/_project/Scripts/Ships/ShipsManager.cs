using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using CREMOT.GameplayUtilities;
using static UnityEngine.GraphicsBuffer;
using System;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

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

        [Space]

        [Header("General Spawn Parameters")]
        [SerializeField] private LayerMask _islandsLayerMask;

        [Header("Spawn Ships Parameters")]
        [SerializeField] private float _spawnDistanceCheck = 10f;
        [SerializeField] private float _checkRadius = 5f;



        [Header("Spawn Objective Parameters")]
        [SerializeField] private SO_ShipObjectivesData _objectiveData;

        [Space]

        [Header("Other")]
        [SerializeField] private bool _isMainMenu;

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

            if (!_isMainMenu)
            {
                DefineNewRandomTimeSpawn();
            }
        }

        private void OnDestroy()
        {
            foreach (ShipBehaviour ship in _ships)
            {
                ship.OnShipDestroyed -= ReactOnDestroyShip;
                ship.OnShipValidated -= ReactOnValidateShip;
                ship.OnShipValidatedWithoutObjective -= ReactOnValidateShipWithoutObjective;
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
            ship.OnShipValidatedWithoutObjective += ReactOnValidateShipWithoutObjective;

            _frequencyToShip[ship.AssociatedFrequency] = ship;
        }

        public void RemoveShip(ShipBehaviour ship)
        {
            if (ship == null)
                return;

            _ships.Remove(ship);

            ship.OnShipDestroyed -= ReactOnDestroyShip;
            ship.OnShipValidated -= ReactOnValidateShip;
            ship.OnShipValidatedWithoutObjective -= ReactOnValidateShipWithoutObjective;

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

            if (!_isMainMenu)
            {
                HandleShipsSpawn();
            }
        }

        #region Ships Spawn
        private void HandleShipsSpawn()
        {
            if (!_enabledShipsSpawn)
                return;

            _currentTimeSpawn += Time.deltaTime;

            if (_currentTimeSpawn >= _randomTimeSpawn && _ships.Count < _data.MaxShips)
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
                //Debug.Log($"Iteration {iteration}", this);
                RaycastHit2D hit = Physics2D.CircleCast(randomPos, _checkRadius, dir, _spawnDistanceCheck, _islandsLayerMask);
                //RaycastHit2D hit = Physics2D.Raycast(randomPos, dir, _spawnDistanceCheck, _islandsLayerMask);

                DrawCircleCast(randomPos, _checkRadius, dir, _spawnDistanceCheck, Color.red, 2f);

                if (hit.collider != null)
                {
                    //Debug.Log($"Hit : {hit.collider.gameObject.name}", hit.collider.gameObject);

                    SpawnShip(++iteration);
                    return;
                }
            }

            if (PoolManager.Instance != null)
            {
                GameObject shipObject = PoolManager.Instance.ActivateShip(randomPos, rot, true);

                ShipBehaviour ship = shipObject.GetComponent<IShipBehaviour>().GetShip();

                ObjectiveShipBehaviour objective = SpawnObjective(randomPos);

                //Debug.Log($"Spawn ship : {ship.name}", ship);
                //Debug.Log($"Spawn Objective : {objective.name}", objective);

                objective.Init();
                ship.SetAssociatedObjective(objective);
                ship.Init();
            }
        }
        #endregion

        #region Objective ship spawn
        private ObjectiveShipBehaviour SpawnObjective(Vector3 shipSpawnPos, int iteration = 0)
        {
            if (PoolManager.Instance == null)
                return null;

            Vector3 centerPos = Camera.main.transform.position;

            Vector2 randomPos = Vector2.zero;

            Vector2 centerSpawnZone = Vector2.zero;

            if (shipSpawnPos.x >= centerPos.x)
            {
                randomPos.x = UnityEngine.Random.Range(-_screenBounds.x + _objectiveData.OffsetFromBordersSpawnObjective, centerPos.x);
                centerSpawnZone.x = (centerPos.x - _screenBounds.x + (_objectiveData.OffsetFromBordersSpawnObjective / 2f)) / 2f;
            }
            else
            {
                randomPos.x = UnityEngine.Random.Range(centerPos.x, _screenBounds.x - _objectiveData.OffsetFromBordersSpawnObjective);
                centerSpawnZone.x = (centerPos.x + _screenBounds.x - (_objectiveData.OffsetFromBordersSpawnObjective / 2f)) / 2f;
            }

            if (shipSpawnPos.y >= centerPos.y)
            {
                randomPos.y = UnityEngine.Random.Range(-_screenBounds.y + _objectiveData.OffsetFromBordersSpawnObjective, centerPos.y);
                centerSpawnZone.y = (centerPos.y - _screenBounds.y + (_objectiveData.OffsetFromBordersSpawnObjective / 2f)) / 2f;
            }
            else
            {
                randomPos.y = UnityEngine.Random.Range(centerPos.y, _screenBounds.y - _objectiveData.OffsetFromBordersSpawnObjective);
                centerSpawnZone.y = (centerPos.y + _screenBounds.y - (_objectiveData.OffsetFromBordersSpawnObjective / 2f)) / 2f;
            }

            DrawRect(centerSpawnZone, _screenBounds.x - (_objectiveData.OffsetFromBordersSpawnObjective / 2f), _screenBounds.y - (_objectiveData.OffsetFromBordersSpawnObjective / 2f), Color.yellow, 2f);

            if (iteration < 10)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(randomPos, _objectiveData.RadiusCheckSpawnObjective, _islandsLayerMask);

                DrawCircle(randomPos, _objectiveData.RadiusCheckSpawnObjective, Color.magenta, 2f);

                if (colliders.Length > 0)
                {
                    return SpawnObjective(shipSpawnPos, ++iteration);
                }
            }

            GameObject objectiveObject = PoolManager.Instance.ActivateObjectiveShip(randomPos, Quaternion.identity, true);

            ObjectiveShipBehaviour objective = objectiveObject.GetComponent<ObjectiveShipBehaviour>();

            return objective;
        }

        #endregion

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

        public void DrawRect(Vector2 center, float width, float height, Color color, float duration = 0f)
        {
            Vector2 topLeft = center - Vector2.right * (width / 2f) + Vector2.up * (height / 2f);
            Vector2 topRight = center + Vector2.right * (width / 2f) + Vector2.up * (height / 2f);
            Vector2 bottomRight = center + Vector2.right * (width / 2f) - Vector2.up * (height / 2f);
            Vector2 bottomLeft = center - Vector2.right * (width / 2f) - Vector2.up * (height / 2f);

            Debug.DrawLine(topLeft, topRight, color, duration);
            Debug.DrawLine(topRight, bottomRight, color, duration);
            Debug.DrawLine(bottomRight, bottomLeft, color, duration);
            Debug.DrawLine(bottomLeft, topLeft, color, duration);
        }

        #endregion

        #region Handle Validate / Destroy
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
            ship.OnShipValidatedWithoutObjective -= ReactOnValidateShipWithoutObjective;

            OnValidateShip?.Invoke();
        }

        private void ReactOnValidateShipWithoutObjective(ShipBehaviour ship)
        {
            ship.OnShipValidated -= ReactOnValidateShip;
            ship.OnShipValidatedWithoutObjective -= ReactOnValidateShipWithoutObjective;

        }
        #endregion

        #region Handle Frequency
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
        #endregion
    }
}
