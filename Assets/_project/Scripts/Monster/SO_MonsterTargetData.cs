using UnityEngine;

namespace AltCtrl.Charybdis
{
    [CreateAssetMenu(fileName = "SO_MonsterTargetData", menuName = "ScriptableObjects/Datas/Monster/SO_MonsterTargetData", order = 2)]
    public class SO_MonsterTargetData : ScriptableObject
    {
        // ----- FIELDS ----- //
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        // ----- FIELDS ----- //

        // ----- PROPERTIES ----- //
        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        // ----- PROPERTIES ----- //
    }
}
