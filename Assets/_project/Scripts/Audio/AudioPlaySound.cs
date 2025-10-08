using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class AudioPlaySound : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("References")]
        [SerializeField] private string _soundName;
        [SerializeField] private bool _playOnEnable;
        [SerializeField] private bool _isLooping = false;
        // ----- FIELDS ----- //

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                PlaySound();
            }
        }

        public void PlaySound()
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySound(_soundName, _isLooping);
            }
        }

        public void StopLoopingSound()
        {
            if (!_isLooping) return;

            if (AudioManager.Instance)
            {
                AudioManager.Instance.StopLoopingSound(_soundName);
            }
        }
    }
}
