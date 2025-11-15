using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_TutoData", menuName = "ScriptableObjects/Datas/Tuto/SO_TutoData", order = 1)]
    public class SO_TutoData : ScriptableObject
    {
        #region Fields
        [Header("Parameters")]
        [SerializeField] private float _showTutoDuration = 7f;

        #endregion

        #region Properties
        public float ShowTutoDuration { get => _showTutoDuration; set => _showTutoDuration = value; }


        #endregion

    }
}
