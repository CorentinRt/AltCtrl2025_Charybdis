using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class DeactivateAfterTime : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Values")]
        [SerializeField] private float _timeToWait;
        // ----- FIELDS ----- //

        private void OnEnable()
        {
            StartCoroutine(Deactivate());
        }

        private void OnDisable()
        {
            StopCoroutine(Deactivate());
        }

        private IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(_timeToWait);
            gameObject.SetActive(false);
        }
    }
}
