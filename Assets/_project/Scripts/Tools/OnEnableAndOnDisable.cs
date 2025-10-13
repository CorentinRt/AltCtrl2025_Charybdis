using UnityEngine;
using UnityEngine.Events;

namespace AltCtrl.Charybdis
{
    public class OnEnableAndOnDisable : MonoBehaviour
    {
        // ----- FIELDS ----- //
        public UnityEvent OnEnableEvent;
        public UnityEvent OnDisableEvent;
        // ----- FIELDS ----- //

        private void OnEnable()
        {
            OnEnableEvent?.Invoke(); 
        }

        private void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }
    }
}
