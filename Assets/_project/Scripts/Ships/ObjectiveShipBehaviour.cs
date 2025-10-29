using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class ObjectiveShipBehaviour : MonoBehaviour
    {
        #region Fields
        [Header("Components")]
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Transform _visualAnchor;

        [Header("Disapear")]
        [SerializeField] private Ease _disappearEase;
        [SerializeField] private float _disappearDuration;

        private Vector3 _startVisualsScale;

        private Tween _disappearTween;

        #endregion

        #region Properties


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
            if (_disappearTween != null)
            {
                _disappearTween.Kill();
            }

            _collider.enabled = true;
            _visualAnchor.localScale = _startVisualsScale;
        }



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
            _collider.enabled = false;

            if (_disappearTween != null)
            {
                _disappearTween.Kill();
            }

            _disappearTween = _visualAnchor.DOScale(0f, _disappearDuration).SetEase(_disappearEase).OnComplete(() =>
            {
                ReactEndDisappear();
            });
        }

        private void ReactEndDisappear()
        {
            gameObject.SetActive(false);
        }
    }
}
