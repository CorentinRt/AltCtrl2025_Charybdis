using UnityEngine;

namespace AltCtrl.Charybdis
{

    [RequireComponent(typeof(Collider2D))]
    public class Storm : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null || collision.gameObject == null)
                return;

            if (collision.gameObject.CompareTag("Ship"))
            {
                collision.gameObject.GetComponent<IShipBehaviour>().GetShip().SetAffectedByStorm(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == null || collision.gameObject == null)
                return;

            if (collision.gameObject.CompareTag("Ship"))
            {
                collision.gameObject.GetComponent<IShipBehaviour>().GetShip().SetAffectedByStorm(false);
            }
        }
    }
}
