using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class ObjectiveShipBehaviour : MonoBehaviour
    {
        #region Fields
        [Header("Data")]
        [SerializeField] private SO_ShipObjectivesData _data;

        [Header("Components")]
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Transform _visualAnchor;

        [Header("Appear")]
        [SerializeField] private AnimationCurve _appearCurve;
        [SerializeField] private float _appearDuration;

        [Header("Disapear")]
        [SerializeField] private AnimationCurve _disappearCurve;
        [SerializeField] private float _disappearDuration;

        [Header("Customization")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Dev anims")]
        [SerializeField] private Transform _objectiveLimit;
        [SerializeField] private float _idleRotateDuration;
        [SerializeField] private Ease _idleRotateEase;

        // Associated Color
        private Color _associatedColor;

        private Vector3 _startVisualsScale;

        private Tween _scaleAnimTween;

        private Tween _idleRotateTween;

        #endregion

        #region Properties
        public Color AssociatedColor => _associatedColor;


        #endregion

        private void Awake()
        {
            _startVisualsScale = _visualAnchor.localScale;
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            InitGlobalScale(TweakableOptionsManager.Exist ? TweakableOptionsManager.Instance.GetScaleCheckpoints() : _data.CheckpointScaleMultiplier);

            if (_scaleAnimTween != null)
            {
                _scaleAnimTween.Kill();
            }

            _collider.enabled = true;
            _visualAnchor.localScale = Vector3.zero;

            PlayAppearAnim();
            PlayIdleRotateAnim();
        }

        private void InitGlobalScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }

        #region Ship reactions Appear / Disappear
        [Button]
        public void ReactGetByShip()
        {
            Disappear();
        }

        [Button]
        public void ReactTargetDestroyed()
        {
            Disappear();
        }

        [Button]
        private void Disappear()
        {
            StopIdleRotateAnim();

            _collider.enabled = false;

            StopScaleAnimTween();

            _scaleAnimTween = _visualAnchor.DOScale(0f, _disappearDuration).SetEase(_disappearCurve).OnComplete(() =>
            {
                ReactEndDisappear();
            });
        }

        private void ReactEndDisappear()
        {
            gameObject.SetActive(false);
        }

        private void StopScaleAnimTween()
        {
            if (_scaleAnimTween != null)
            {
                _scaleAnimTween.Kill();
                _scaleAnimTween = null;
            }
        }

        private void PlayAppearAnim()
        {
            StopScaleAnimTween();

            _scaleAnimTween = _visualAnchor.DOScale(_startVisualsScale, _appearDuration).SetEase(_appearCurve);
        }

        #endregion

        #region Objectives customization
        public void SetObjectivesColor(Color color)
        {
            _spriteRenderer.material.color = color;
        }

        public void SetObjectivesMotif(Texture2D texture)
        {
            if (_spriteRenderer.material.HasTexture("_Objective_texture"))
            {
                _spriteRenderer.material.SetTexture("_Objective_texture", texture);
            }
        }

        #endregion

        #region Dev anim
        private void PlayIdleRotateAnim()
        {
            StopIdleRotateAnim();

            _objectiveLimit.rotation = Quaternion.identity;

            _idleRotateTween = _objectiveLimit.DOLocalRotate(new Vector3(0f, 0f, 360f), _idleRotateDuration, RotateMode.FastBeyond360).SetEase(_idleRotateEase).OnComplete(() =>
            {
                PlayIdleRotateAnim();
            });
        }

        private void StopIdleRotateAnim()
        {
            if (_idleRotateTween != null)
            {
                _idleRotateTween.Kill();
                _idleRotateTween = null;
            }
        }

        #endregion
    }
}
