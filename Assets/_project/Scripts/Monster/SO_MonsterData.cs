using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_MonsterData", menuName = "ScriptableObjects/Datas/Monster/SO_MonsterData", order = 1)]
    public class SO_MonsterData : ScriptableObject
    {
        // ----- FIELDS ----- //
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _maxRotation = 45f;
        [SerializeField] private float _rotateAcceleration = 1f;

        [Header("Typhon")]
        [SerializeField] private float _typhoonCantMoveTime = 3f;
        [SerializeField] private float _typhoonCooldown = 5f;

        [Header("Teleport")]
        [SerializeField] private float _teleportCooldown = 2f;
        // ----- FIELDS ----- //

        // ----- PROPERTIES ----- //
        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public float MaxRotation { get => _maxRotation; set => _maxRotation = value; }
        public float RotateAcceleration { get => _rotateAcceleration; set => _rotateAcceleration = value; }
        public float TyphoonCantMoveTime { get => _typhoonCantMoveTime; set => _typhoonCantMoveTime = value; }
        public float TyphoonCooldown { get => _typhoonCooldown; set => _typhoonCooldown = value; }
        public float TeleportCooldown { get => _teleportCooldown; set => _teleportCooldown = value; }
        // ----- PROPERTIES ----- //
    }
}
