using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AltCtrl.Charybdis
{
    public class MainMenuManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Language")]
        [SerializeField] private LocalizeStringEvent _languageLocalizeStringEvent;

        [Header("Microphone")]
        [SerializeField] private LocalizeStringEvent _microphoneLocalizeStringEvent;

        [Header("Volume")]
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private AudioMixer _audioMixer;
        // ----- FIELDS ----- //

        private void Start()
        {
            UpdateLanguageTxt();
            UpdateMicrophoneTxt();

            SetupVolumeSlider();

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnNextMicrophonePressed += SetNextMicrophone;
                InputManager.Instance.OnNextLanguagePressed += SetNextLanguage;

                InputManager.Instance.OnTotem1Pressed += Play;
                InputManager.Instance.OnTotem2Pressed += Play;
                InputManager.Instance.OnTotem3Pressed += Play;

                _volumeSlider.onValueChanged.AddListener(UpdateAudioMixerVolume);
                InputManager.Instance.OnMoveRadioRotatorPressed += ChangeVolumeWithRadio; 
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnNextMicrophonePressed -= SetNextMicrophone;
                InputManager.Instance.OnNextLanguagePressed -= SetNextLanguage;

                InputManager.Instance.OnTotem1Pressed -= Play;
                InputManager.Instance.OnTotem2Pressed -= Play;
                InputManager.Instance.OnTotem3Pressed -= Play;

                _volumeSlider.onValueChanged.RemoveListener(UpdateAudioMixerVolume);
                InputManager.Instance.OnMoveRadioRotatorPressed -= ChangeVolumeWithRadio;

            }
        }

        #region Language
        private void UpdateLanguageTxt()
        {
            string currentLanguage = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.CultureInfo.NativeName;

            var localizedString = _languageLocalizeStringEvent.StringReference;

            (localizedString["language"] as StringVariable).Value = currentLanguage;

            _languageLocalizeStringEvent.RefreshString();
        }

        private void SetNextLanguage(bool pressed)
        {
            if (!pressed) return;

            var locales = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales;
            var currentLocale = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale;

            int currentIndex = locales.IndexOf(currentLocale);
            int nextIndex = (currentIndex + 1) % locales.Count;

            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale = locales[nextIndex];

            UpdateLanguageTxt();
        }
        #endregion

        #region Microphone
        private void UpdateMicrophoneTxt()
        {
            var localizedString = _microphoneLocalizeStringEvent.StringReference;

            (localizedString["micName"] as StringVariable).Value = InputManager.Instance.GetCurrentOrFirstMicrophone();

            _microphoneLocalizeStringEvent.RefreshString();
        }

        private void SetNextMicrophone(bool pressed)
        {
            if (!pressed) return;

            InputManager.Instance.SetNextMicrophone();
            UpdateMicrophoneTxt();
        }
        #endregion

        #region Play
        public void Play(bool pressed)
        {
            if (!pressed) return;
            Play();
        }

        public void Play()
        {
            SceneManager.LoadScene("MainGame");
        }
        #endregion

        #region Volume
        public void SetupVolumeSlider()
        {
            _volumeSlider.maxValue = 100f;
            _volumeSlider.value = 100f; 
        }

        public void ChangeVolumeWithRadio(int value)
        {
            _volumeSlider.value += value;

            UpdateAudioMixerVolume(_volumeSlider.value);
        }

        public void UpdateAudioMixerVolume(float value)
        {
            float volume = Mathf.Clamp(value, 0.0001f, 100f); // éviter log(0)
            float dB = Mathf.Log10(volume / 100f) * 80f; // dB entre -80 et 0
            _audioMixer.SetFloat("MasterVolume", dB);
        }
        #endregion Volume
    }
}
