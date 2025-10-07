using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class Totem : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("References")]
        [SerializeField] private GameObject _storm;
        // ----- FIELDS ----- //

        private void Start()
        {
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            _storm.SetActive(active);
        }
    }
}
