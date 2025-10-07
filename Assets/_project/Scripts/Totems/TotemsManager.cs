using System.Security.Cryptography;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class TotemsManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Totems")]
        [SerializeField] private Totem _totem1;
        [SerializeField] private Totem _totem2;
        [SerializeField] private Totem _totem3;

        private int _currentlyActiveTotem = -1;
        // ----- FIELDS ----- //

        private void Start()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTotem1Pressed += OnTotem1;
                InputManager.Instance.OnTotem2Pressed += OnTotem2;
                InputManager.Instance.OnTotem3Pressed += OnTotem3;
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTotem1Pressed -= OnTotem1;
                InputManager.Instance.OnTotem2Pressed -= OnTotem2;
                InputManager.Instance.OnTotem3Pressed -= OnTotem3;
            }
        }

        private void OnTotem1(bool pressed)
        {
            Debug.Log(_currentlyActiveTotem);

            if (!pressed)
            {
                _currentlyActiveTotem = -1;
            }
            else
            {
                if (_currentlyActiveTotem != 1 && _currentlyActiveTotem != -1) return;
                _currentlyActiveTotem = 1;
            }

            _totem1.SetActive(pressed);
        }

        private void OnTotem2(bool pressed)
        {
            Debug.Log(_currentlyActiveTotem);

            if (!pressed) 
            {
                _currentlyActiveTotem = -1;
            }
            else 
            {
                if (_currentlyActiveTotem != 2 && _currentlyActiveTotem != -1) return;
                _currentlyActiveTotem = 2;
            }

            _totem2.SetActive(pressed);
        }

        private void OnTotem3(bool pressed)
        {
            Debug.Log(_currentlyActiveTotem);

            if (!pressed)
            {
                _currentlyActiveTotem = -1;
            }
            else
            {
                if (_currentlyActiveTotem != 3 && _currentlyActiveTotem != -1) return;
                _currentlyActiveTotem = 3;
            }

            _totem3.SetActive(pressed);
        }
    }
}
