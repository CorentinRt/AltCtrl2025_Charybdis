using UnityEngine;

namespace AltCtrl.Charybdis
{
    [RequireComponent(typeof(Collider2D))]
    public class MonsterTyphoon : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // to do : check boat - add force toward transform.position (center)
        }
    }
}
