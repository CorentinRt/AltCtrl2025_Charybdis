using UnityEngine;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Collider2D))]
    public class MonsterTyphoon : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null || collision.gameObject == null)
                return;

            if (collision.gameObject.CompareTag("Ship"))
            {
                collision.gameObject.GetComponent<IShipBehaviour>().GetShip().SetAffectedByTyphon(true, transform.position);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == null || collision.gameObject == null)
                return;

            if (collision.gameObject.CompareTag("Ship"))
            {
                collision.gameObject.GetComponent<IShipBehaviour>().GetShip().SetAffectedByTyphon(false, transform.position);
            }
        }
    }
}
