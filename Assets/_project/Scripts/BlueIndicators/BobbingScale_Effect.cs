using DG.Tweening;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class BobbingScale_Effect : MonoBehaviour
    {
        #region Fields
        [SerializeField] private bool _infinite;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;

        [SerializeField] private float _targetScale;

        private Vector3 _startScale;

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
            PlayBobbingEffect();
        }

        private void PlayBobbingEffect(bool invert = false)
        {
            transform.DOScale(invert ? _targetScale : _startScale.x
                , _duration).SetEase(_ease).OnComplete(() =>
            {
                PlayBobbingEffect(!invert);
            });
        }

    }
}
