using CREMOT.GameplayUtilities;
using System;
using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class GameManager : GenericSingleton<GameManager>
    {
        public enum GAME_PHASES
        {
            PRE_GAME = 0,
            IN_GAME = 1,
            POST_GAME = 2
        }

        #region Fields

        [Header("Data")]
        [SerializeField] private SO_GamePhasesData _data;

        private GAME_PHASES _currentGamePhase;

        private Coroutine _preGameCoroutine;

        #endregion

        #region Properties


        #endregion

        public event Action<GAME_PHASES> OnGamePhaseChanged;


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            SetGamePhase(GAME_PHASES.PRE_GAME);
        }

        private void SetGamePhase(GAME_PHASES gamePhase)
        {
            _currentGamePhase = gamePhase;

            switch (_currentGamePhase)
            {
                case GAME_PHASES.PRE_GAME:
                    
                    if (ShipsManager.Exist)
                    {
                        ShipsManager.Instance.SetEnableShipsSpawn(false);
                    }

                    StartPreGameCoroutine();

                    break;

                case GAME_PHASES.IN_GAME:

                    if (ShipsManager.Exist)
                    {
                        ShipsManager.Instance.SetEnableShipsSpawn(true);
                    }

                    break;

                case GAME_PHASES.POST_GAME:

                    if (ShipsManager.Exist)
                    {
                        ShipsManager.Instance.SetEnableShipsSpawn(false);
                    }

                    break;
            }

            OnGamePhaseChanged?.Invoke(_currentGamePhase);
        }

        private void StartPreGameCoroutine()
        {
            if (_preGameCoroutine != null)
                return;

            _preGameCoroutine = StartCoroutine(PreGameCoroutine());
        }

        private void StopPreGameCoroutine()
        {
            if (_preGameCoroutine == null)
                return;

            StopCoroutine(_preGameCoroutine);
            _preGameCoroutine = null;
        }

        private IEnumerator PreGameCoroutine()
        {
            yield return new WaitForSeconds(_data.CooldownSecondsDuration);

            SetGamePhase(GAME_PHASES.IN_GAME);

            StopPreGameCoroutine();
        }
    }
}
