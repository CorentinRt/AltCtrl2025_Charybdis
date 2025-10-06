using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class ShipBehaviour : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;

        [SerializeField] private Transform _visualAnchor;


        [SerializeField] private float _speed;
        [SerializeField] private float _rotateSpeed;
    }
}
