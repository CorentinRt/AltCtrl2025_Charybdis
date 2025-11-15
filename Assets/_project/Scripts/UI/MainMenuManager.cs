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

        [Header("Param")]
        [SerializeField] private bool _autoSpawnShip;

        private ShipBehaviour _spawnedShip;
        // ----- FIELDS ----- //

        private void Start()
        {
            if (_autoSpawnShip)
            {
                SpawnShip();
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTotem1Pressed += Play;
                InputManager.Instance.OnTotem2Pressed += Play;
                InputManager.Instance.OnTotem3Pressed += Play;
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTotem1Pressed -= Play;
                InputManager.Instance.OnTotem2Pressed -= Play;
                InputManager.Instance.OnTotem3Pressed -= Play;
            }
        }

        private void SpawnShip()
        {
            if (!_autoSpawnShip)
                return;

            GameObject newSpawnedShipGO = PoolManager.Instance.ActivateShip(_spawnFishPos.position, _spawnFishPos.rotation);
            _spawnedShip = newSpawnedShipGO.GetComponent<ShipBehaviour>();
            _spawnedShip.Init();
            _spawnedShip.OnShipDestroyed += OnSpawnedFishDestroyed;
            _spawnedShip.OnShipValidated += OnSpawnedFishDestroyed;
        }

        private void OnSpawnedFishDestroyed(ShipBehaviour ship) 
        {
            if (!_autoSpawnShip)
                return;

            _spawnedShip.OnShipDestroyed -= OnSpawnedFishDestroyed;
            _spawnedShip.OnShipValidated -= OnSpawnedFishDestroyed;
            SpawnShip();
        }

        private void Play(bool pressed)
        {
            if (!pressed) return;

            SceneManager.LoadScene("MainGame");
        }
    }
}
