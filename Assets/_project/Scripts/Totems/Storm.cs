using UnityEngine;

namespace AltCtrl.Charybdis
{

    [RequireComponent(typeof(Collider2D))]
    public class Storm : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // check collision bateau -> direction aléatoire
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // check collision bateau -> plus de direction aléatoire
        }
    }
}
