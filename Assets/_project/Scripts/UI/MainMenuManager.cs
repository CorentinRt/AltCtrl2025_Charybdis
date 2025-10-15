using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AltCtrl.Charybdis
{
    public class MainMenuManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("References")]
        [SerializeField] private Transform _spawnFishPos;

        private bool _isShipReady = false;
        private bool _isMonsterReady = false;

        private bool _isWaitingForLaunch = false;
        private Coroutine _launchGameCoroutine;

        private ShipBehaviour _spawnedShip;
        // ----- FIELDS ----- //

        private void Start()
        {
            SpawnShip();
        }

        public void SetShipReady()
        {
            Debug.Log("ship ready");
            _isShipReady = true;
        }

        public void SetShipNotReady()
        {
            Debug.Log("ship not ready");
            _isShipReady = false;
        }

        public void SetMonsterReady()
        {
            Debug.Log("monster ready");
            _isMonsterReady = true;
        }

        public void SetMonsterNotReady()
        {
            Debug.Log("monster not ready");
            _isMonsterReady = false;
        }

        private void CheckBothPlayersReady()
        {
            if (_isShipReady &&  _isMonsterReady && !_isWaitingForLaunch)
            {
                Debug.Log("both ready");
                _isWaitingForLaunch = true;
                // countdown 3 2 1
                _launchGameCoroutine = StartCoroutine(WaitAndLaunchGame());
            }
            else
            {
                if (_launchGameCoroutine != null)
                    StopCoroutine(_launchGameCoroutine);

                _isWaitingForLaunch = false;
            }
        }

        private IEnumerator WaitAndLaunchGame()
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("MainGame");
        }

        private void SpawnShip()
        {
            GameObject newSpawnedShipGO = PoolManager.Instance.ActivateShip(_spawnFishPos.position, _spawnFishPos.rotation);
            _spawnedShip = newSpawnedShipGO.GetComponent<ShipBehaviour>();
            _spawnedShip.Init();
            _spawnedShip.OnShipDestroyed += OnSpawnedFishDestroyed;
            _spawnedShip.OnShipValidated += OnSpawnedFishDestroyed;
        }

        private void OnSpawnedFishDestroyed(ShipBehaviour ship) 
        {
            _spawnedShip.OnShipDestroyed -= OnSpawnedFishDestroyed;
            _spawnedShip.OnShipValidated -= OnSpawnedFishDestroyed;
            SpawnShip();
        }
    }
}
