using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class TestLoopingSound : MonoBehaviour
    {
        private void OnEnable()
        {
            AudioManager.Instance.PlaySound("Test", true);
            StartCoroutine(WaitAndStopSound());
        }

        private IEnumerator WaitAndStopSound()
        {
            yield return new WaitForSeconds(5f);
            AudioManager.Instance.StopLoopingSound("Test");
        }
    }
}
