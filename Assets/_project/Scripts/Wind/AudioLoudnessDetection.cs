using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class AudioLoudnessDetection : MonoBehaviour
    {
        // ----- FIELDS ----- //
        private int _sampleWindow = 64;

        private AudioClip _microphoneClip;
        // ----- FIELDS ----- //

        private void Start()
        {
            MicrophoneToAudioClip();
        }

        public void MicrophoneToAudioClip()
        {
            _microphoneClip = Microphone.Start(InputManager.Instance.GetCurrentOrFirstMicrophone(), true, 1, AudioSettings.outputSampleRate);
        }

        public float GetLoudnessFromMicrophone()
        {
            return GetLoudnessFromAudioClip(Microphone.GetPosition(InputManager.Instance.GetCurrentOrFirstMicrophone()), _microphoneClip);
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
