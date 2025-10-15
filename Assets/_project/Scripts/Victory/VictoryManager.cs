using CREMOT.GameplayUtilities;
using System;
using System.Runtime.CompilerServices;
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
        [SerializeField] private float _add1MultiplierEachTime = 10f;

        private float _currentMultiplier = 1f;
        private float _currentWaitTime = 0f;

        private bool _godHasWon;
        private bool _humanHasWon;

        [Header("References")]
        [SerializeField] private Slider _victorySlider; // HUMAN 0 - GOD MAX
        // ----- FIELDS ----- //

        public bool HasWon => _humanHasWon || _godHasWon;
        public bool GodHasWon => _godHasWon;
        public bool HumanHasWon => _humanHasWon;

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
                InputManager.Instance.OnTotem1Pressed += CheckReturnToMainMenu;
                InputManager.Instance.OnTotem2Pressed += CheckPlayAgain;
                InputManager.Instance.OnTotem3Pressed += CheckReturnToMainMenu;
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
                InputManager.Instance.OnTotem1Pressed -= CheckReturnToMainMenu;
                InputManager.Instance.OnTotem2Pressed -= CheckPlayAgain;
                InputManager.Instance.OnTotem3Pressed -= CheckReturnToMainMenu;
            }
        }

        private void Update()
        {
            _currentWaitTime += Time.deltaTime;

            if (_currentWaitTime > _add1MultiplierEachTime)
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
            if (HasWon)
                return;

            _victorySlider.value += _addGodScoreOnDestroyShip * _currentMultiplier;

            _victorySlider.value = Mathf.Clamp(_victorySlider.value, 0, _sliderMaxValue);

            CheckVictory();
        }

        private void AddHumanScoreOnDestroyShip()
        {
            if (HasWon)
                return;

            _victorySlider.value -= _addHumanScoreOnLeaveShip * _currentMultiplier;

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

        private void CheckReturnToMainMenu(bool pressed)
        {
            if (!pressed || !HasWon)
                return;

            SceneManager.LoadScene("MainMenu");
        }

        private void CheckPlayAgain(bool pressed)
        {
            if (!pressed || !HasWon)
                return;

            SceneManager.LoadScene("MainGame");
        }
    }
}
