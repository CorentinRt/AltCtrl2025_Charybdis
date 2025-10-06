using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class AudioLoudnessDetection : MonoBehaviour
    {
        // ----- FIELDS ----- //
        private int _sampleWindow = 64;

        private string _microphoneName;
        private AudioClip _microphoneClip;
        // ----- FIELDS ----- //

        private void Start()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("Aucun microphone détecté !");
                return;
            }

            // Get the first microphone in devices list
            _microphoneName = Microphone.devices[0];
            Debug.Log("Microphone utilisé : " + _microphoneName);

            MicrophoneToAudioClip();
        }

        public void MicrophoneToAudioClip()
        {
            _microphoneClip = Microphone.Start(_microphoneName, true, 1, AudioSettings.outputSampleRate);
        }

        public float GetLoudnessFromMicrophone()
        {
            return GetLoudnessFromAudioClip(Microphone.GetPosition(_microphoneName), _microphoneClip);
        }

        public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
        {
            int startPosition = clipPosition - _sampleWindow;

            if (startPosition < 0) return 0;

            float[] waveData = new float[_sampleWindow];
            clip.GetData(waveData, startPosition);

            // Compute loudness 
            float totalLoudness = 0;
            for (int i = 0; i < _sampleWindow; i++)
            {
                totalLoudness += Mathf.Abs(waveData[i]);
            }

            return totalLoudness / _sampleWindow;
        }
    }
}
