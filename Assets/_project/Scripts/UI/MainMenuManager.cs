using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AltCtrl.Charybdis
{
    public class MainMenuManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("References")]
        [SerializeField] private Transform _spawnFishPos;

        [Space]

        [Header("Menu Islands Circles")]
        [SerializeField] private List<MenuSelectableIsland> _menuIslandsCircle;
        [SerializeField] private int _startMenuNavigationIndex = 0;
        [SerializeField] private float _delayBetweenEachSelection = 1f;

        [Space]

        [Header("Param")]
        [SerializeField] private bool _autoSpawnShip;
        [SerializeField] private bool _useTotemButtonsToTriggerMennuButtons;

        private ShipBehaviour _spawnedShip;

        // Nav Menu
        int _currentMenuNavigationIndex = 0;
        float _currentTimeBeforeSelectionAvailable = 0f;

        // ----- FIELDS ----- //

        private void Start()
        {
            // Auto spawn single ship
            if (_autoSpawnShip)
            {
                SpawnShip();
            }

            // Handle Navigation start index
            if (_menuIslandsCircle.Count > 0)
            {
                _currentMenuNavigationIndex = Mathf.Clamp(_startMenuNavigationIndex, 0, _menuIslandsCircle.Count - 1);
            }

            // Handle bindings inputs
            if (InputManager.Instance != null)
            {
                // for navigation with totem buttons
                if (_useTotemButtonsToTriggerMennuButtons)
                {
                    InputManager.Instance.OnTotem1Pressed += Play;
                    InputManager.Instance.OnTotem2Pressed += StartTuto;
                    InputManager.Instance.OnTotem3Pressed += Play;
                }
            }

            if (MenuNavigationManager.Instance != null)
            {
                MenuNavigationManager.Instance.OnTriggerEffectSelectableIslands += ReactOnMenuNavigationManagerTriggered;
            }

        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                if (_useTotemButtonsToTriggerMennuButtons)
                {
                    InputManager.Instance.OnTotem1Pressed -= Play;
                    InputManager.Instance.OnTotem2Pressed -= StartTuto;
                    InputManager.Instance.OnTotem3Pressed -= Play;
                }
            }

            if (MenuNavigationManager.Instance != null)
            {
                MenuNavigationManager.Instance.OnTriggerEffectSelectableIslands -= ReactOnMenuNavigationManagerTriggered;
            }
        }

        #region Spawn Ship Menu
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
        #endregion

        #region Play & Start Tuto
        private void Play(bool pressed)
        {
            if (!pressed) return;

            SceneManager.LoadScene("MainGame");
        }

        private void StartTuto(bool pressed)
        {
            if (!pressed)
                return;

            SceneManager.LoadScene("Human_Tuto");
        }
        #endregion

        private void ReactOnMenuNavigationManagerTriggered(MenuSelectableIsland.SELECTABLE_EFFECT effect)
        {
            switch (effect)
            {
                case MenuSelectableIsland.SELECTABLE_EFFECT.None:
                    break;

                case MenuSelectableIsland.SELECTABLE_EFFECT.PLAY:
                    Play(true);
                    break;

                case MenuSelectableIsland.SELECTABLE_EFFECT.START_TUTO:
                    StartTuto(true);
                    break;

                case MenuSelectableIsland.SELECTABLE_EFFECT.RETURN_MENU:
                    break;

                default:
                    break;
            }
        }
    }
}
