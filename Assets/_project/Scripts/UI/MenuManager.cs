using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AltCtrl.Charybdis
{
    public class MenuManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Language")]
        [SerializeField] private LocalizeStringEvent _languageLocalizeStringEvent;
        private Coroutine _changeLanguageCouroutine;

        [Header("Microphone")]
        [SerializeField] private LocalizeStringEvent _microphoneLocalizeStringEvent;
        private Coroutine _changeMicrophoneCouroutine;

        [Header("Volume")]
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private AudioMixer _audioMixer;

        [Header("Pause")]
        [SerializeField] private bool _isPauseMenu = false;
        [SerializeField] private GameObject _pauseGameObject;
        private bool _isInPause = false;

        [Header("Values")]
        [SerializeField] private float _showDebugTextsTime = 2f;
        // ----- FIELDS ----- //

        private void Start()
        {
            if (_pauseGameObject != null) _pauseGameObject.SetActive(false);

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

                if (_isPauseMenu)
                {
                    InputManager.Instance.OnPausePressed += TogglePauseMenu;
                }
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

                if (_isPauseMenu)
                {
                    InputManager.Instance.OnPausePressed -= TogglePauseMenu;
                }

            }
        }

        #region Language
        private void UpdateLanguageTxt()
        {
            string currentLanguage = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.CultureInfo.NativeName;

            var localizedString = _languageLocalizeStringEvent.StringReference;

            (localizedString["language"] as StringVariable).Value = currentLanguage;

            _languageLocalizeStringEvent.RefreshString();

            if (_changeLanguageCouroutine != null) StopCoroutine(_changeLanguageCouroutine);
            _changeLanguageCouroutine = StartCoroutine(ShowAndHideLanguage());
        }

        private void SetNextLanguage(bool pressed)
        {
            if (!pressed) return;
            if (_isPauseMenu && !_isInPause) return;

            var locales = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales;
            var currentLocale = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale;

            int currentIndex = locales.IndexOf(currentLocale);
            int nextIndex = (currentIndex + 1) % locales.Count;

            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale = locales[nextIndex];

            UpdateLanguageTxt();
        }

        private IEnumerator ShowAndHideLanguage()
        {
            yield return new WaitForSecondsRealtime(0.2f); // temps refresh string
            _languageLocalizeStringEvent.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(_showDebugTextsTime);
            _languageLocalizeStringEvent.gameObject.SetActive(false);
        }
        #endregion

        #region Microphone
        private void UpdateMicrophoneTxt()
        {
            var localizedString = _microphoneLocalizeStringEvent.StringReference;

            (localizedString["micName"] as StringVariable).Value = InputManager.Instance.GetCurrentOrFirstMicrophone();

            _microphoneLocalizeStringEvent.RefreshString();

            if (_changeMicrophoneCouroutine != null) StopCoroutine(_changeMicrophoneCouroutine);
            _changeMicrophoneCouroutine = StartCoroutine(ShowAndHideMicrophone());
        }

        private void SetNextMicrophone(bool pressed)
        {
            if (!pressed) return;
            if (_isPauseMenu && !_isInPause) return;

            InputManager.Instance.SetNextMicrophone();
            UpdateMicrophoneTxt();
        }

        private IEnumerator ShowAndHideMicrophone()
        {
            yield return new WaitForSecondsRealtime(0.2f); // temps refresh string
            _microphoneLocalizeStringEvent.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(_showDebugTextsTime);
            _microphoneLocalizeStringEvent.gameObject.SetActive(false);
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
            if (_isPauseMenu && !_isInPause) return;

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

        #region Pause
        public void TogglePauseMenu(bool pressed)
        {
            if (!pressed) return;

            _isInPause = !_isInPause;

            if (_isInPause)
            {
                Time.timeScale = 0f;
                _pauseGameObject.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                _pauseGameObject.SetActive(false);
            }
        }

        public void ReturnToGame()
        {
            TogglePauseMenu(true);
        }
        #endregion
    }
}
