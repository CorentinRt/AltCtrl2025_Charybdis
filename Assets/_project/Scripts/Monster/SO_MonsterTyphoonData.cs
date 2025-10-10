using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_MonsterTyphoonData", menuName = "ScriptableObjects/Datas/Monster/SO_MonsterTyphoonData", order = 2)]
    public class SO_MonsterTyphoonData : ScriptableObject
    {
        // ----- FIELDS ----- //
        [Header("Animation")]
        [SerializeField] private float _activationAnimTime = 2f;
        [SerializeField] private float _deactivationAnimTime = 3f;

        [Header("Lifetime")]
        [SerializeField] private float _timeBeforeDeactivate = 8f;
        // ----- FIELDS ----- //

        // ----- PROPERTIES ----- //
        public float ActivationAnimTime { get => _activationAnimTime; set => _activationAnimTime = value; }
        public float DeactivationAnimTime { get => _deactivationAnimTime; set => _deactivationAnimTime = value; }
        public float TimeBeforeDeactivate { get => _timeBeforeDeactivate; set => _timeBeforeDeactivate = value; }
        // ----- PROPERTIES ----- //
    }
}
