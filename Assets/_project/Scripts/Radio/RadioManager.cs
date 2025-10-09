using CREMOT.GameplayUtilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class RadioManager : GenericSingleton<RadioManager>
    {
        #region Fields
        [Header("Data")]
        [SerializeField] private SO_RadioData _data;

        private List<(int, int)> _allFrequencies = new();

        private HashSet<(int, int)> _usedFrequencies = new();

        private int _currentIndexFrequency;

        private float _currentTimeKeyPressed;

        private bool _pressedRadioInputThisFrame;
        private int _radioInputDir;

        private float _canRadioInputCooldown;

        private bool _inputEnable;

        #endregion

        #region Properties


        #endregion

        public event Action<(int, int)> OnChangeFrequency;


        protected override void Awake()
        {
            base.Awake();

            Init();
        }

        private void Init()
        {
            for (int i = _data.MainFrequencyMin; i <= _data.MainFrequencyMax; ++i)
            {
                for (int j = _data.ExternalFrequencyMin; j <= _data.ExternalFrequencyMax; ++j)
                {
                    _allFrequencies.Add((i, j));
                }
            }
        }

        private void Start()
        {
            SetFrequencyIndex(UnityEngine.Random.Range(0, _allFrequencies.Count));

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMoveRadioRotatorPressed += OnMoveRadioRotateInput;
            }
        }
        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMoveRadioRotatorPressed -= OnMoveRadioRotateInput;
            }
        }

        private void OnMoveRadioRotateInput(int dir)
        {
            if (!_inputEnable)
                return;

            if (_canRadioInputCooldown > 0f)
            {
                return;
            }

            _pressedRadioInputThisFrame = true;
            _radioInputDir = dir;
        }

        private void Update()
        {
            if (_canRadioInputCooldown >= 0f)
            {
                _canRadioInputCooldown -= Time.deltaTime;
            }

            if (_pressedRadioInputThisFrame)
            {
                if (_radioInputDir > 0)
                {
                    GoNextFrequency();
                }
                else
                {
                    GoPreviousFrequency();
                }

                if (ShipsManager.Exist)
                {
                    if (ShipsManager.Instance.CheckShipWithFrequencyExist(_allFrequencies[_currentIndexFrequency]))
                    {
                        _canRadioInputCooldown = _data.TimeToHoldIfHasShipControlled;
                    }
                    else
                    {
                        _canRadioInputCooldown = _data.TimeToHoldIfNoShipControlled;
                    }
                }

                _pressedRadioInputThisFrame = false;
            }
        }

        private void GoNextFrequency()
        {
            if (_currentIndexFrequency + 1 >= _allFrequencies.Count)
                return;

            SetFrequencyIndex(_currentIndexFrequency + 1);
        }

        private void GoPreviousFrequency()
        {
            if (_currentIndexFrequency - 1 < 0)
                return;

            SetFrequencyIndex(_currentIndexFrequency - 1);
        }

        private void SetFrequencyIndex(int index)
        {
            if (index >= _allFrequencies.Count || index < 0)
                return;

            _currentIndexFrequency = index;

            OnChangeFrequency?.Invoke(_allFrequencies[_currentIndexFrequency]);
        }

        public (int, int) GetCurrentFrequency()
        {
            return _allFrequencies[_currentIndexFrequency];
        }

        public (int, int) GetNewAvailableFrequency()
        {
            if (_usedFrequencies.Count == _allFrequencies.Count)
                return (-1, -1);

            int randomIndex = UnityEngine.Random.Range(0, _allFrequencies.Count);

            while (_usedFrequencies.Contains(_allFrequencies[randomIndex]))
            {
                randomIndex++;
            }

            return _allFrequencies[randomIndex];
        }

        public void AddFrequencyUsed((int, int) frequency)
        {
            _usedFrequencies.Add(frequency);
        }

        public void RemoveFrequencyUsed((int, int) frequency)
        {
            _usedFrequencies.Remove(frequency);
        }

        public void SetEnableRadioInput(bool enable)
        {
            _inputEnable = enable;
        }
    }
}
