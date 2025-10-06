using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class WindDetector : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioLoudnessDetection _detector;

        [SerializeField] private float _loudnessSensibility = 100f;
        [SerializeField] private float _loudnessTreshold = 0.1f;
        // + min & max scale
        // ----- FIELDS ----- //

        private void Update()
        {
            float loudness = _detector.GetLoudnessFromMicrophone() * _loudnessSensibility;

            if (loudness < _loudnessTreshold) loudness = 0;

            Debug.Log($"Loudness : {loudness}");
        }
    }
}
