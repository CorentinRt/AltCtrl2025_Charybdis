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
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _currentTimeKeyPressed += Time.deltaTime;

                if (_currentTimeKeyPressed >= _data.TimeToHoldIfNoShipControlled)
                {
                    _currentTimeKeyPressed = 0f;

                    GoPreviousFrequency();
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _currentTimeKeyPressed += Time.deltaTime;

                if (_currentTimeKeyPressed >= _data.TimeToHoldIfNoShipControlled)
                {
                    _currentTimeKeyPressed = 0f;

                    GoNextFrequency();
                }
            }
            else
            {
                _currentTimeKeyPressed = 0f;
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
    }
}
