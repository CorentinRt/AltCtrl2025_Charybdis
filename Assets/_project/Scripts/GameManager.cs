using CREMOT.GameplayUtilities;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AltCtrl.Charybdis
{
    public class GameManager : GenericSingleton<GameManager>
    {
        public enum GAME_PHASES
        {
            PRE_GAME = 0,
            IN_GAME = 1,
            POST_GAME = 2,
            TUTO_SHOW = 3
        }

        #region Fields

        [Header("Data")]
        [SerializeField] private SO_GamePhasesData _data;
        [SerializeField] private SO_TutoData _tutoData;

        [Header("Start Phase")]
        [SerializeField] private GAME_PHASES _startingPhase = GAME_PHASES.PRE_GAME;

        [Header("Post game")]
        [SerializeField] private bool _redirectToSceneWhenPostGame;
        [ShowIf("_redirectToSceneWhenPostGame")]
        [SerializeField] private float _cooldownBeforeRedirecting = 1f;
        [ShowIf("_redirectToSceneWhenPostGame")]
        [SerializeField] private string _nameSceneRedirect;

        private GAME_PHASES _currentGamePhase;

        private Coroutine _preGameCoroutine;
        private Coroutine _tutoShowCoroutine;
        private Coroutine _redirectCoroutine;

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
            if (VictoryManager.Exist)
            {
                VictoryManager.Instance.OnVictory += ReactOnVictory;
            }

            SetGamePhase(_startingPhase);
        }

        private void OnDestroy()
        {
            if (VictoryManager.Exist)
            {
                VictoryManager.Instance.OnVictory -= ReactOnVictory;
            }
        }

        private void ReactOnVictory()
        {
            if (_currentGamePhase == GAME_PHASES.POST_GAME)
                return;

            SetGamePhase(GAME_PHASES.POST_GAME);
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

                    if (Monster.Exist)
                    {
                        Monster.Instance.SetEnableMonsterMovement(false);
                    }

                    if (MonsterTarget.Exist)
                    {
                        MonsterTarget.Instance.SetEnableMonsterTargetInput(false);
                    }

                    if (RadioManager.Exist)
                    {
                        RadioManager.Instance.SetEnableRadioInput(false);
                    }

                    StartPreGameCoroutine();

                    break;

                case GAME_PHASES.IN_GAME:

                    if (ShipsManager.Exist)
                    {
                        ShipsManager.Instance.SetEnableShipsSpawn(true);
                    }

                    if (Monster.Exist)
                    {
                        Monster.Instance.SetEnableMonsterMovement(true);
                    }

                    if (MonsterTarget.Exist)
                    {
                        MonsterTarget.Instance.SetEnableMonsterTargetInput(true);
                    }

                    if (RadioManager.Exist)
                    {
                        RadioManager.Instance.SetEnableRadioInput(true);
                    }

                    break;

                case GAME_PHASES.POST_GAME:
                    if (ShipsManager.Exist)
                    {
                        ShipsManager.Instance.SetEnableShipsSpawn(false);

                        if (VictoryManager.Exist)
                        {
                            if (VictoryManager.Instance.GodHasWon)
                            {
                                ShipsManager.Instance.DestroyAllShips();
                            }
                            else if (VictoryManager.Instance.HumanHasWon)
                            {
                                ShipsManager.Instance.ValidateAllShip();
                            }
                        }
                    }

                    if (Monster.Exist)
                    {
                        Monster.Instance.SetEnableMonsterMovement(false);
                    }

                    if (MonsterTarget.Exist)
                    {
                        MonsterTarget.Instance.SetEnableMonsterTargetInput(false);
                    }

                    if (RadioManager.Exist)
                    {
                        RadioManager.Instance.SetEnableRadioInput(false);
                    }

                    if (_redirectToSceneWhenPostGame)
                    {
                        StartRedirectCoroutine();
                    }

                    break;
                case GAME_PHASES.TUTO_SHOW:
                    if (ShipsManager.Exist)
                    {
                        ShipsManager.Instance.SetEnableShipsSpawn(false);
                    }

                    if (Monster.Exist)
                    {
                        Monster.Instance.SetEnableMonsterMovement(false);
                    }

                    if (MonsterTarget.Exist)
                    {
                        MonsterTarget.Instance.SetEnableMonsterTargetInput(false);
                    }

                    if (RadioManager.Exist)
                    {
                        RadioManager.Instance.SetEnableRadioInput(false);
                    }

                    StartTutoShowCoroutine();
                    break;
            }

            OnGamePhaseChanged?.Invoke(_currentGamePhase);
        }

        #region Pre game
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
            yield return new WaitForSeconds(_data.CooldownSecondsDuration + 1); // +1 to let 0 text display

            SetGamePhase(GAME_PHASES.IN_GAME);

            StopPreGameCoroutine();
        }
        #endregion

        #region Tuto show
        private void StartTutoShowCoroutine()
        {
            if (_tutoShowCoroutine != null)
                return;

            _tutoShowCoroutine = StartCoroutine(TutoShowCoroutine());
        }

        private void StopTutoShowCoroutine()
        {
            if (_preGameCoroutine == null)
                return;

            StopCoroutine(_tutoShowCoroutine);
            _tutoShowCoroutine = null;
        }

        private IEnumerator TutoShowCoroutine()
        {
            if (_tutoData != null)
            {
                yield return new WaitForSeconds(_tutoData.ShowTutoDuration + 1);
            }
            else
            {
                Debug.LogError("Error : no tuto data in game manager but tried to start tuto show phase !!!", this);
            }

            SetGamePhase(GAME_PHASES.PRE_GAME);

            StopTutoShowCoroutine();
        }
        #endregion

        #region Post game redirect
        private void StartRedirectCoroutine()
        {
            if (_redirectCoroutine != null)
                return;

            _redirectCoroutine = StartCoroutine(RedirectCoroutine());
        }

        private void StopRedirectCoroutine()
        {
            if (_redirectCoroutine == null)
                return;

            StopCoroutine(_redirectCoroutine);
            _redirectCoroutine = null;
        }

        private IEnumerator RedirectCoroutine()
        {
            yield return new WaitForSeconds(_cooldownBeforeRedirecting + 1);

            SceneManager.LoadScene(_nameSceneRedirect);

            StopRedirectCoroutine();
        }
        #endregion
    }
}
