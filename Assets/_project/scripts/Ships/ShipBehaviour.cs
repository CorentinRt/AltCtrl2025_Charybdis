using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class ShipBehaviour : MonoBehaviour
    {
        #region Fields
        [Header("Components")]
        [SerializeField] private Rigidbody2D _rb;

        [SerializeField] private Transform _visualAnchor;

        [Space]

        [Header("Datas")]
        [SerializeField] private SO_ShipData _data;

        #endregion


        #region Properties


        #endregion


        private void Move()
        {
            float deltaInput = Input.GetAxis("Horizontal");

            if (deltaInput != 0f)
            {

            }


        }

    }
}
