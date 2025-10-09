using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class UI_PreGame : MonoBehaviour
    {
        #region Fields
        [Header("Data")]
        [SerializeField] private SO_GamePhasesData _data;

        [SerializeField] private TextMeshProUGUI _countdownLabel;

        private Coroutine _countdownPreGameCoroutine;

        #endregion

        #region Properties


        #endregion

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (GameManager.Exist)
            {
                GameManager.Instance.OnGamePhaseChanged += ReactOnGamePhaseChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Exist)
            {
                GameManager.Instance.OnGamePhaseChanged -= ReactOnGamePhaseChanged;
            }
        }

        private void ReactOnGamePhaseChanged(GameManager.GAME_PHASES gamePhase)
        {
            switch (gamePhase)
            {
                case GameManager.GAME_PHASES.PRE_GAME:
                    StartCountdownVisual();
                    break;

                case GameManager.GAME_PHASES.IN_GAME:
                    StopCountDownVisual();
                    break;

                case GameManager.GAME_PHASES.POST_GAME:
                    StopCountDownVisual();
                    break;
            }
        }

        private void StartCountdownVisual()
        {
            if (_countdownPreGameCoroutine != null)
                return;

            _countdownPreGameCoroutine = StartCoroutine(CountdownPreGameCoroutine());
        }

        private void StopCountDownVisual()
        {
            if (_countdownPreGameCoroutine != null)
            {
                StopCoroutine(_countdownPreGameCoroutine);
                _countdownPreGameCoroutine = null;
            }

            _countdownLabel.gameObject.SetActive(false);
        }

        private IEnumerator CountdownPreGameCoroutine()
        {
            for (int i = 0; i <= _data.CooldownSecondsDuration; ++i)
            {
                _countdownLabel.text = $"{_data.CooldownSecondsDuration - i}";

                _countdownLabel.transform.rotation = Quaternion.identity;

                _countdownLabel.transform.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.8f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);

                yield return new WaitForSeconds(1f);
            }

            StopCountDownVisual();
        }

    }
}
