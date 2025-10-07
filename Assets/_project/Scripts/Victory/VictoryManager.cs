using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace AltCtrl.Charybdis
{
    public class VictoryManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _sliderMaxValue = 10f;
        [SerializeField] private float _addGodScoreOnDestroyShip = 1;
        [SerializeField] private float _addHumanScoreOnLeaveShip = 1;

        [Header("References")]
        [SerializeField] private Slider _victorySlider; // HUMAN 0 - GOD MAX

        // TODO : + de score en fonction du temps qui passe
        // ----- FIELDS ----- //

        private void Start()
        {
            SetupSlider();
        }
        private void SetupSlider()
        {
            if (_victorySlider == null) return;

            _victorySlider.maxValue = _sliderMaxValue;
            _victorySlider.value = _sliderMaxValue / 2;
        }

        private void AddGodScoreOnDestroyShip()
        {
            _victorySlider.value += _addGodScoreOnDestroyShip;

            _victorySlider.value = Mathf.Clamp(_victorySlider.value, 0, _sliderMaxValue);

            CheckVictory();
        }

        private void AddHumanScoreOnDestroyShip()
        {
            _victorySlider.value -= _addHumanScoreOnLeaveShip;

            _victorySlider.value = Mathf.Clamp(_victorySlider.value, 0, _sliderMaxValue);

            CheckVictory();
        }

        private void CheckVictory()
        {
            if (_victorySlider.value == _sliderMaxValue)
            {
                Debug.Log("HUMAN VICTORY");
            }
        }
    }
}
