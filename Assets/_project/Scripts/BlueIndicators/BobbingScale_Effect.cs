using DG.Tweening;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class BobbingScale_Effect : MonoBehaviour
    {
        #region Fields
        [Header("Param effect")]
        [SerializeField] private bool _infinite;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;
        [SerializeField] private float _targetScale;

        [Header("Play on start")]
        [SerializeField] private bool _playOnStart = false;

        private Vector3 _startScale;

        private Tween _bobbingEffectTween;

        #endregion

        #region Properties


        #endregion

        private void Start()
        {
            _startScale = transform.localScale;

            Init();
        }

        private void Init()
        {
            transform.localScale = Vector3.zero;

            if (_playOnStart)
            {
                PlayBobbingEffect(false);
            }
            else
            {
                if (_bobbingEffectTween == null)
                {
                    _bobbingEffectTween = transform.DOScale(_startScale.x
                    , _duration).SetEase(_ease);
                }
            }
        }

        public void PlayBobbingEffect(bool invert = false)
        {
            StopBobbingEffect();

            _bobbingEffectTween = transform.DOScale(invert ? _targetScale : _startScale.x
                , _duration).SetEase(_ease).OnComplete(() =>
            {
                PlayBobbingEffect(!invert);
            });
        }

        public void StopBobbingEffect(bool resetScale = false)
        {
            if (_bobbingEffectTween != null)
            {
                _bobbingEffectTween.Kill(false);
                _bobbingEffectTween = null;
            }

            if (resetScale)
            {
                _bobbingEffectTween = transform.DOScale(_startScale.x, _duration).SetEase(_ease);
            }
        }

    }
}
