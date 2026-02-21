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

                // To confirm effect on current selected menu island circle
                InputManager.Instance.OnWindPressed += TriggerCurrentMenuIslandCircleEffect;
                InputManager.Instance.OnMoveShipRotatorPressed += HandleHumanMoveRotatorInput;
            }

            SelectNewIslandCircle(_currentMenuNavigationIndex, -1);
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

                InputManager.Instance.OnWindPressed -= TriggerCurrentMenuIslandCircleEffect;
                InputManager.Instance.OnMoveShipRotatorPressed -= HandleHumanMoveRotatorInput;
            }
        }

        private void Update()
        {
            UpdateDelayBetweenEachSelection();
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

        #region Navigation Menu Islands Cirlce

        #region Trigger Menu Islands Effects
        private void TriggerMenuIslandCircleEffectByIndex(int index)
        {
            if (_menuIslandsCircle.Count == 0 || index >= _menuIslandsCircle.Count)
            {
                Debug.LogError("Error : Try triggering a Menu islands effect but List of Menu Islands circle isn't set in MenuManager, or index is too high !", this);
                return;
            }

            MenuSelectableIsland selectedIsland = _menuIslandsCircle[_currentMenuNavigationIndex];

            if (selectedIsland == null)
                return;

            switch (selectedIsland.GetEffectType())
            {
                case MenuSelectableIsland.SELECTABLE_EFFECT.None:
                    break;

                case MenuSelectableIsland.SELECTABLE_EFFECT.PLAY:
                    Play(true);
                    break;

                case MenuSelectableIsland.SELECTABLE_EFFECT.START_TUTO:
                    StartTuto(true);
                    break;

                default:
                    break;
            }
        }

        private void TriggerCurrentMenuIslandCircleEffect(bool pressed)
        {
            TriggerMenuIslandCircleEffectByIndex(_currentMenuNavigationIndex);
        }
        #endregion

        #region Islands Selection
        private void UpdateDelayBetweenEachSelection()
        {
            if (_currentTimeBeforeSelectionAvailable > 0f)
            {
                _currentTimeBeforeSelectionAvailable -= Time.deltaTime;
            }
        }

        private bool CanSelectNewIsland()
        {
            return _currentTimeBeforeSelectionAvailable <= 0f;
        }

        private void HandleHumanMoveRotatorInput(int radioAmplitude)
        {
            if (radioAmplitude > 0)
            {
                SelectNextIslandCircle();
            }
            else if (radioAmplitude < 0)
            {
                SelectPreviousIslandCircle();
            }
        }

        private void SelectNewIslandCircle(int newIndex, int oldIndex)
        {
            if (_menuIslandsCircle.Count == 0)
            {
                Debug.LogError("Error : Try selecting a new Menu islands but List of Menu Islands circle isn't set in MenuManager !", this);
                return;
            }

            if (!CanSelectNewIsland())
                return;

            _currentTimeBeforeSelectionAvailable = _delayBetweenEachSelection;

            if (newIndex < 0)
                newIndex = _menuIslandsCircle.Count - 1;

            _currentMenuNavigationIndex = newIndex % _menuIslandsCircle.Count;

            // Enable visual effect selection new island
            MenuSelectableIsland selectedIsland = _menuIslandsCircle[_currentMenuNavigationIndex];

            if (selectedIsland != null)
            {
                selectedIsland.NotifyOnStartSelectionHover();            
            }

            // Disable visual effect selection old island
            if (oldIndex >= 0 && oldIndex < _menuIslandsCircle.Count)
            {
                MenuSelectableIsland oldSelectedIsland = _menuIslandsCircle[oldIndex];

                if (oldSelectedIsland != null)
                {
                    oldSelectedIsland.NotifyOnEndSelectionHover();
                }
            }
        }

        private void SelectNextIslandCircle()
        {
            SelectNewIslandCircle(_currentMenuNavigationIndex + 1, _currentMenuNavigationIndex);
        }
        private void SelectPreviousIslandCircle()
        {
            SelectNewIslandCircle(_currentMenuNavigationIndex - 1, _currentMenuNavigationIndex);
        }
        #endregion

        #endregion
    }
}
