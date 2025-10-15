using UnityEngine;
using UnityEngine.Events;

namespace AltCtrl.Charybdis
{
    public class OnTriggerEnterAndExit : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private string _checkTag = "";

        public UnityEvent OnTriggerEnter;
        public UnityEvent OnTriggerExit;
        // ----- FIELDS ----- //

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_checkTag != "")
            {
                if (collision.CompareTag(_checkTag))
                {
                    OnTriggerEnter?.Invoke();
                }
            }
            else
            {
                OnTriggerEnter?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_checkTag != "")
            {
                if (collision.CompareTag(_checkTag))
                {
                    OnTriggerExit?.Invoke();
                }
            }
            else
            {
                OnTriggerExit?.Invoke();
            }
        }
    }
}
