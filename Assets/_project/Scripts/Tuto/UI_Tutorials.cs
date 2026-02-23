using DG.Tweening;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class UI_Tutorials : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private GameObject _concernedTutoUI;
        [SerializeField] private GameObject _containerPopupTuto;

        [Header("Anim param")]
        [SerializeField] private float _disappearDuration = 1f;
        [SerializeField] private Ease _disappearEase;
        [SerializeField] private bool _playAnimWithGamePhase = true;

        private Tween _scaleDisappearTween;
        #endregion

        #region Properties


        #endregion

        private void Awake()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePhaseChanged += ReactGamePhaseChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Exist)
            {
                GameManager.Instance.OnGamePhaseChanged -= ReactGamePhaseChanged;
            }
        }

        private void ReactGamePhaseChanged(GameManager.GAME_PHASES gamePhase)
        {
            switch (gamePhase)
            {
                case GameManager.GAME_PHASES.PRE_GAME:
                case GameManager.GAME_PHASES.IN_GAME:
                case GameManager.GAME_PHASES.POST_GAME:
                    if (_playAnimWithGamePhase)
                    {
                        HideTuto();
                    }
                    break;
                case GameManager.GAME_PHASES.TUTO_SHOW:
                    if (_playAnimWithGamePhase)
                    {
                        ShowTuto();
                    }
                    break;
            }
        }

        public void ShowTuto()
        {
            StopScaleDisappearAnim();

            _concernedTutoUI.SetActive(true);
        }

        public void HideTuto()
        {
            if (!_concernedTutoUI.activeSelf)
                return;

            StopScaleDisappearAnim();

            _scaleDisappearTween = _containerPopupTuto.transform.DOScale(0f, _disappearDuration).SetEase(_disappearEase).OnComplete(() =>
            {
                _concernedTutoUI.SetActive(false);
            });
        }

        private void StopScaleDisappearAnim()
        {
            if (_scaleDisappearTween != null)
            {
                _scaleDisappearTween.Kill();
                _scaleDisappearTween = null;
            }
        }

    }
}
