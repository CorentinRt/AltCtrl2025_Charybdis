using DG.Tweening;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_MonsterData", menuName = "ScriptableObjects/Datas/Monster/SO_MonsterData", order = 1)]
    public class SO_MonsterData : ScriptableObject
    {
        // ----- FIELDS ----- //
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;

        [Header("Typhon")]
        [SerializeField] private float _typhoonCantMoveTime = 3f;
        [SerializeField] private float _typhoonCooldown = 5f;
        [SerializeField] private float _indicationBeforeTyphoon = 2f;

        [Header("Spawn dev anim")]
        [SerializeField] private float _durationSpawn;
        [SerializeField] private Ease _easingSpawn;
        // ----- FIELDS ----- //

        // ----- PROPERTIES ----- //
        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public float TyphoonCantMoveTime { get => _typhoonCantMoveTime; set => _typhoonCantMoveTime = value; }
        public float IndicationBeforeTyphoon { get => _indicationBeforeTyphoon; set => _indicationBeforeTyphoon = value; }
        public float TyphoonCooldown { get => _typhoonCooldown; set => _typhoonCooldown = value; }
        public float DurationSpawn { get => _durationSpawn; set => _durationSpawn = value; }
        public Ease EasingSpawn { get => _easingSpawn; set => _easingSpawn = value; }
        // ----- PROPERTIES ----- //
    }
}
