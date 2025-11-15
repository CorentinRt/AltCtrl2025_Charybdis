using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.WSA;

namespace AltCtrl.Charybdis
{
    public class UI_PreGame : MonoBehaviour
    {
        #region Fields
        [Header("Data")]
        [SerializeField] private SO_GamePhasesData _data;

        [Header("References")]
        [SerializeField] private GameObject _holder;
        [SerializeField] private TextMeshProUGUI _countdownLabel;

        [Header("Appear anim")]
        [SerializeField] private float _appearDuration;
        [SerializeField] private Ease _appearEase;


        private Coroutine _countdownPreGameCoroutine;

        #endregion

        #region Properties


        #endregion

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _holder.SetActive(false);

            if (GameManager.Instance != null)
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
                case GameManager.GAME_PHASES.TUTO_SHOW:
                    StopCountDownVisual();
                    break;
            }
        }

        private void StartCountdownVisual()
        {
            if (_countdownPreGameCoroutine != null)
                return;

            _countdownLabel.text = $"{_data.CooldownSecondsDuration}";

            _holder.transform.localScale = Vector3.zero;
            _holder.SetActive(true);
            _holder.transform.DOScale(1f, _appearDuration).SetEase(_appearEase);

            _countdownPreGameCoroutine = StartCoroutine(CountdownPreGameCoroutine());
        }

        private void StopCountDownVisual()
        {
            if (_countdownPreGameCoroutine != null)
            {
                StopCoroutine(_countdownPreGameCoroutine);
                _countdownPreGameCoroutine = null;
            }

            _holder.SetActive(false);
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
