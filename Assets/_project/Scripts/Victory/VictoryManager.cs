using CREMOT.GameplayUtilities;
using System;
using System.Runtime.CompilerServices;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AltCtrl.Charybdis
{
    public class VictoryManager : GenericSingleton<VictoryManager>
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _sliderMaxValue = 20f;
        [SerializeField] private float _addGodScoreOnDestroyShip = 1;
        [SerializeField] private float _addHumanScoreOnLeaveShip = 1;
        [SerializeField] private SO_VictoryData _data;

        private float _currentMultiplier = 1f;
        private float _currentWaitTime = 0f;

        private bool _godHasWon;
        private bool _humanHasWon;

        [Header("References")]
        [SerializeField] private Slider _victorySlider; // HUMAN 0 - GOD MAX

        [Header("Victory param (for tuto)")]
        [SerializeField] private bool _preventGodVictory;
        [SerializeField] private bool _preventHumanVictory;
        [SerializeField] private bool _preventManualChangeScene;

        [Header("Use Totem for buttons")]
        [SerializeField] private bool _useTotemButtonsToTriggerMennuButtons = false;
        // ----- FIELDS ----- //

        public bool HasWon => _humanHasWon || _godHasWon;
        public bool GodHasWon => _godHasWon;
        public bool HumanHasWon => _humanHasWon;

        public bool PreventGodVictory { get => _preventGodVictory; set => _preventGodVictory = value; }
        public bool PreventHumanVictory { get => _preventHumanVictory; set => _preventHumanVictory = value; }

        public event Action OnGodVictory;
        public event Action OnHumanVictory;
        public event Action OnVictory;


        private void Start()
        {
            if (_victorySlider == null) return;

            SetupSlider();

            if (ShipsManager.Exist)
            {
                ShipsManager.Instance.OnDestroyShip += AddGodScoreOnDestroyShip;
                ShipsManager.Instance.OnValidateShip += AddHumanScoreOnDestroyShip;
            }

            if (InputManager.Instance != null)
            {
                if (_useTotemButtonsToTriggerMennuButtons)
                {
                    InputManager.Instance.OnTotem1Pressed += CheckPlayAgain;
                    InputManager.Instance.OnTotem2Pressed += CheckReturnToMainMenu;
                    InputManager.Instance.OnTotem3Pressed += CheckPlayAgain;
                }
            }

            if (MenuNavigationManager.Instance != null)
            {
                MenuNavigationManager.Instance.OnTriggerEffectSelectableIslands += ReactOnMenuNavigationManagerTriggered;
            }
        }

        private void OnDestroy()
        {
            if (_victorySlider == null) return;

            if (ShipsManager.Exist)
            {
                ShipsManager.Instance.OnDestroyShip -= AddGodScoreOnDestroyShip;
                ShipsManager.Instance.OnValidateShip -= AddHumanScoreOnDestroyShip;
            }

            if (InputManager.Instance != null)
            {
                if (_useTotemButtonsToTriggerMennuButtons)
                {
                    InputManager.Instance.OnTotem1Pressed -= CheckPlayAgain;
                    InputManager.Instance.OnTotem2Pressed -= CheckReturnToMainMenu;
                    InputManager.Instance.OnTotem3Pressed -= CheckPlayAgain;
                }
            }

            if (MenuNavigationManager.Instance != null)
            {
                MenuNavigationManager.Instance.OnTriggerEffectSelectableIslands -= ReactOnMenuNavigationManagerTriggered;
            }
        }

        private void Update()
        {
            _currentWaitTime += Time.deltaTime;

            if (_currentWaitTime > _data.Add1MultiplierEachTime)
            {
                _currentMultiplier++;
                _currentWaitTime = 0f;
            }
        }

        private void SetupSlider()
        {
            _victorySlider.maxValue = _sliderMaxValue;
            _victorySlider.value = _sliderMaxValue / 2;
        }

        private void AddGodScoreOnDestroyShip()
        {
            if (HasWon || _preventGodVictory)
                return;

            float personalMultiplier = TweakableOptionsManager.Exist ? TweakableOptionsManager.Instance.GetGodPointsFactor() : _data.GodPersonalMultiplier;

            _victorySlider.value += personalMultiplier * _addGodScoreOnDestroyShip * _currentMultiplier;

            _victorySlider.value = Mathf.Clamp(_victorySlider.value, 0, _sliderMaxValue);

            CheckVictory();
        }

        private void AddHumanScoreOnDestroyShip()
        {
            if (HasWon || _preventHumanVictory)
                return;

            float personalMultiplier = TweakableOptionsManager.Exist ? TweakableOptionsManager.Instance.GetHumansPointsFactor() : _data.GodPersonalMultiplier;

            _victorySlider.value -= personalMultiplier * _addHumanScoreOnLeaveShip * _currentMultiplier;

            _victorySlider.value = Mathf.Clamp(_victorySlider.value, 0, _sliderMaxValue);

            CheckVictory();
        }

        private void CheckVictory()
        {
            if (HasWon)
                return;

            if (_victorySlider.value == _sliderMaxValue)
            {
                OnGodVictory?.Invoke();
                _godHasWon = true;
                OnVictory?.Invoke();
            }
            else if (_victorySlider.value == 0f)
            {
                OnHumanVictory?.Invoke();
                _humanHasWon = true;
                OnVictory?.Invoke();
            }
        }

        private void ReactOnMenuNavigationManagerTriggered(MenuSelectableIsland.SELECTABLE_EFFECT effect)
        {
            switch (effect)
            {
                case MenuSelectableIsland.SELECTABLE_EFFECT.None:
                    break;

                case MenuSelectableIsland.SELECTABLE_EFFECT.PLAY:
                    CheckPlayAgain(true);
                    break;

                case MenuSelectableIsland.SELECTABLE_EFFECT.START_TUTO:
                    break;

                case MenuSelectableIsland.SELECTABLE_EFFECT.RETURN_MENU:

                    CheckReturnToMainMenu(true);
                    break;

                default:
                    break;
            }
        }

        private bool CanPlayAgain()
        {
            if (!HasWon || _preventManualChangeScene)
                return false;

            return true;
        }

        private bool CanReturnToMainMenu()
        {
            if (!HasWon || _preventManualChangeScene)
                return false;

            return true;
        }

        private void CheckReturnToMainMenu(bool pressed)
        {
            if (!pressed || !CanReturnToMainMenu())
                return;

            SceneManager.LoadScene("MainMenu");
        }

        private void CheckPlayAgain(bool pressed)
        {
            if (!pressed || !CanPlayAgain())
                return;

            SceneManager.LoadScene("MainGame");
        }
    }
}
