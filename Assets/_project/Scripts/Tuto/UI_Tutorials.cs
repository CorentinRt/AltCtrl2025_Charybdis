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

        private float _startScale;

        private Tween _scaleDisappearTween;
        #endregion

        #region Properties


        #endregion

        private void Awake()
        {
            _startScale = _containerPopupTuto.transform.localScale.x;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePhaseChanged += ReactGamePhaseChanged;
            }

            _containerPopupTuto.transform.localScale = Vector3.zero;
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
                        TriggerAnimTuto("Start");
                    }
                    break;
            }
        }

        public void ShowTuto()
        {
            _concernedTutoUI.SetActive(true);

            StopScaleDisappearAnim();

            _scaleDisappearTween = _containerPopupTuto.transform.DOScale(_startScale, _disappearDuration).SetEase(_disappearEase);

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

        public void TriggerAnimTuto(string triggerName)
        {
            if (!_concernedTutoUI)
            {
                Debug.LogWarning("Error : Try to go to tuto part, but no concerned tuto set in UI_Tutorials !", this);
                return;
            }

            if (_concernedTutoUI.TryGetComponent<Animator>(out Animator animator))
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                Debug.LogWarning("Error : No animator found on UI_Tutorials concerned tuto !", this);
            }
        }
    }
}
