using CREMOT.GameplayUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class MenuNavigationManager : GenericSingleton<MenuNavigationManager>
    {
        #region Fields
        [Header("Menu Islands Circles")]
        [SerializeField] private List<MenuSelectableIsland> _menuIslandsCircle;
        [SerializeField] private int _startMenuNavigationIndex = 0;
        [SerializeField] private float _delayBetweenEachSelection = 1f;

        [SerializeField] private float _delayAfterInitBeforeEnablingSelectionEffect = 3f;

        // Nav Menu
        int _currentMenuNavigationIndex = 0;
        float _currentTimeBeforeSelectionAvailable = 0f;

        float _currentDelayAfterInitBeforeEnablingSelectionEffect;
        #endregion


        #region Properties


        #endregion

        public event Action<MenuSelectableIsland.SELECTABLE_EFFECT> OnTriggerEffectSelectableIslands;

        protected override void Awake()
        {
            _currentDelayAfterInitBeforeEnablingSelectionEffect = _delayAfterInitBeforeEnablingSelectionEffect;
        }

        private void Start()
        {
            // Handle bindings inputs
            if (InputManager.Instance != null)
            {
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
                InputManager.Instance.OnWindPressed -= TriggerCurrentMenuIslandCircleEffect;
                InputManager.Instance.OnMoveShipRotatorPressed -= HandleHumanMoveRotatorInput;
            }
        }

        private void Update()
        {
            UpdateDelayBetweenEachSelection();

            if (_currentDelayAfterInitBeforeEnablingSelectionEffect > 0f)
            {
                _currentDelayAfterInitBeforeEnablingSelectionEffect -= Time.deltaTime;
            }
        }

        #region Navigation Menu Islands Cirlce

        #region Trigger Menu Islands Effects
        private void TriggerMenuIslandCircleEffectByIndex(int index)
        {
            if (_menuIslandsCircle.Count == 0 || index >= _menuIslandsCircle.Count)
            {
                Debug.LogError("Error : Try triggering a Menu islands effect but List of Menu Islands circle isn't set in MenuManager, or index is too high !", this);
                return;
            }

            if (_currentDelayAfterInitBeforeEnablingSelectionEffect > 0f)
                return;

            MenuSelectableIsland selectedIsland = _menuIslandsCircle[_currentMenuNavigationIndex];

            if (selectedIsland == null)
                return;

            OnTriggerEffectSelectableIslands?.Invoke(selectedIsland.GetEffectType());
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
