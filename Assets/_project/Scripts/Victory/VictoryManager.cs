using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace AltCtrl.Charybdis
{
    public class VictoryManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _sliderMaxValue = 20f;
        [SerializeField] private float _addGodScoreOnDestroyShip = 1;
        [SerializeField] private float _addHumanScoreOnLeaveShip = 1;
        [SerializeField] private float _add1MultiplierEachTime = 10f;

        private float _currentMultiplier = 1f;
        private float _currentWaitTime = 0f;

        [Header("References")]
        [SerializeField] private Slider _victorySlider; // HUMAN 0 - GOD MAX
        // ----- FIELDS ----- //

        private void Start()
        {
            if (_victorySlider == null) return;

            SetupSlider();

            if (ShipsManager.Instance != null)
            {
                ShipsManager.Instance.OnDestroyShip += AddGodScoreOnDestroyShip;
                ShipsManager.Instance.OnValidateShip += AddHumanScoreOnDestroyShip;
            }
        }

        private void OnDestroy()
        {
            if (_victorySlider == null) return;

            if (ShipsManager.Instance != null)
            {
                ShipsManager.Instance.OnDestroyShip -= AddGodScoreOnDestroyShip;
                ShipsManager.Instance.OnValidateShip -= AddHumanScoreOnDestroyShip;
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
            _victorySlider.value += _addGodScoreOnDestroyShip * _currentMultiplier;

            _victorySlider.value = Mathf.Clamp(_victorySlider.value, 0, _sliderMaxValue);

            CheckVictory();
        }

        private void AddHumanScoreOnDestroyShip()
        {
            _victorySlider.value -= _addHumanScoreOnLeaveShip * _currentMultiplier;

            _victorySlider.value = Mathf.Clamp(_victorySlider.value, 0, _sliderMaxValue);

            CheckVictory();
        }

        private void CheckVictory()
        {
            if (_victorySlider.value == _sliderMaxValue)
            {
                Debug.Log("GOD VICTORY");
            }
            else
            {
                Debug.Log("HUMAN VICTORY");
            }
        }
    }
}
