using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    [Serializable]
    public struct AudioInfo
    {
        public string Name;
        public AudioClip Clip;
        [Range(0, 1)]
        public float Volume;
    }

    public class AudioManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        public static AudioManager Instance;

        [Header("Sounds")]
        [SerializeField] private List<AudioInfo> _soundList = new List<AudioInfo>();

        private Dictionary<string, AudioInfo> _soundDict = new Dictionary<string, AudioInfo>();
        private Dictionary<string, AudioSource> _loopingSources = new Dictionary<string, AudioSource>();
        private Dictionary<string, Transform> _originalParents = new Dictionary<string, Transform>();
        // ----- FIELDS ----- //

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Populate the dictionary
            foreach (AudioInfo info in _soundList)
            {
                if (!_soundDict.ContainsKey(info.Name))
                {
                    //Debug.Log($"Sound dict : {info.Name}");
                    _soundDict.Add(info.Name, info);
                }
                else
                    Debug.LogWarning($"Duplicate sound name in AudioManager: {info.Name}");
            }
        }

        public void PlaySound(string soundName, bool isLooping = false, float pitch = 1.0f)
        {
            Debug.Log($"Play 2D sound: {soundName}");

            if (!TryGetSound(soundName, out AudioInfo audioInfo))
                return;

            if (isLooping && _loopingSources.ContainsKey(soundName))
                return; // Already playing

            AudioSource source = FindUnusedAudioSource();
            if (!source) return;

            SetupNewSound(source, audioInfo, isLooping, soundName, pitch);
        }

        public void StopLoopingSound(string soundName)
        {
            if (_loopingSources.TryGetValue(soundName, out AudioSource source))
            {
                source.Stop();
                source.clip = null;
                source.loop = false;

                // Reset parent if saved
                if (_originalParents.TryGetValue(soundName, out Transform originalParent))
                {
                    //Debug.Log($"Set source {source.name} parent : {originalParent.name}");
                    source.transform.SetParent(originalParent);
                    source.transform.localPosition = Vector3.zero;
                    _originalParents.Remove(soundName);
                }

                _loopingSources.Remove(soundName);
            }
            else
            {
                Debug.LogWarning($"No looping sound found for: {soundName}");
            }
        }

        private IEnumerator WaitSoundEndAndResetParent(Transform initialParent, Transform audioSource, float audioTime)
        {
            yield return new WaitForSeconds(audioTime);
            if (audioSource != null)
            {
                //Debug.Log($"Set source {audioSource.name} parent : {initialParent.name}");
                audioSource.SetParent(initialParent);
                audioSource.transform.position = Vector3.zero;
            }
        }

        private AudioSource FindUnusedAudioSource()
        {
            return PoolManager.Instance.GetAudioSource();
        }

        private bool TryGetSound(string name, out AudioInfo audio)
        {
            if (_soundDict.TryGetValue(name, out audio))
                return true;

            Debug.LogError($"AudioManager: Sound '{name}' not found in list.");
            return false;
        }

        private void SetupNewSound(AudioSource source, AudioInfo info, bool isLooping, string name, float pitch = 1f)
        {
            source.clip = info.Clip;
            source.volume = info.Volume;
            source.pitch = pitch;

            source.spatialBlend = 0f;
            source.transform.position = Vector3.zero;

            source.loop = isLooping;
            if (isLooping)
                _loopingSources[name] = source;


            source.Play();
        }
    }
}
