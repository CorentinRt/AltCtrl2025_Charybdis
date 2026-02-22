using DG.Tweening;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class MenuSelectableIsland : MonoBehaviour
    {
        public enum SELECTABLE_EFFECT
        {
            None = 0,
            PLAY = 1,
            START_TUTO = 2,
            RETURN_MENU = 3
        }

        #region Fields
        [Header("References")]
        [SerializeField] private Transform _selectableVisuals;
        [SerializeField] private SpriteRenderer _selectableSpriteRenderer;

        [Header("Selectable Effect")]
        [SerializeField] private SELECTABLE_EFFECT _selectableEffect;

        [Header("Visual effect selection")]
        [SerializeField] private Color _selectedColor;
        [SerializeField] private float _colorTransitionDuration = 1f;
        [SerializeField] private Ease _selectedColorEase;

        [SerializeField] private float _selectedTargetScale = 1f;
        [SerializeField] private float _scaleTransitionDuration = 1f;
        [SerializeField] private Ease _selectedScaleEase;

        private Tween _selectionColorEffectTween;
        private Tween _selectionScaleEffectTween;

        private float _startScale;
        private Color _startColor;

        #endregion

        #region Properties


        #endregion

        private void Awake()
        {
            _startScale = transform.localScale.x;
            _startColor = _selectableSpriteRenderer.color;
        }

        public SELECTABLE_EFFECT GetEffectType()
        {
            return _selectableEffect;
        }

        public void NotifyOnStartSelectionHover()
        {
            StopSelectionEffectTween();

            _selectionColorEffectTween = _selectableSpriteRenderer.DOColor(_selectedColor, _colorTransitionDuration).SetEase(_selectedColorEase);

            _selectionScaleEffectTween = _selectableVisuals.DOScale(_selectedTargetScale, _scaleTransitionDuration).SetEase(_selectedScaleEase);
        }

        public void NotifyOnEndSelectionHover()
        {
            StopSelectionEffectTween();

            _selectionColorEffectTween = _selectableSpriteRenderer.DOColor(_startColor, _colorTransitionDuration).SetEase(_selectedColorEase);

            _selectionScaleEffectTween = _selectableVisuals.DOScale(_startScale, _scaleTransitionDuration).SetEase(_selectedScaleEase);
        }

        private void StopSelectionEffectTween()
        {
            if (_selectionColorEffectTween == null)
            {
                _selectionColorEffectTween.Kill();
                _selectionColorEffectTween = null;
            }

            if (_selectionScaleEffectTween == null)
            {
                _selectionScaleEffectTween.Kill(true);
                _selectionScaleEffectTween = null;
            }
        }
    }
}
