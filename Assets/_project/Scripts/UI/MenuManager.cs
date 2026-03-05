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

        [Space(20)]

        [Header("Tweaking Game Options")]
        [SerializeField] private SO_ShipsManagerData _shipsManagerData;
        [SerializeField] private SO_ShipData _shipData;
        [SerializeField] private SO_ShipObjectivesData _shipObjectivesData;
        [SerializeField] private SO_MonsterData _monsterData;
        [SerializeField] private SO_VictoryData _victoryData;

        [SerializeField] private Slider _maxShipNumberSlider;
        [SerializeField] private Slider _checkpointScaleSlider;
        [SerializeField] private Slider _maxSpeedShipsSlider;
        [SerializeField] private Slider _minSpeedShipsSlider;
        [SerializeField] private Slider _maxSpeedMonsterSlider;
        [SerializeField] private Slider _minTimeBeforeShipSpawnSlider;
        [SerializeField] private Slider _maxTimeBeforeShipSpawnSlider;
        [SerializeField] private Slider _godPointsFactorSlider;
        [SerializeField] private Slider _humansPointsFactorSlider;


        // ----- FIELDS ----- //

        private void Start()
        {
            if (_pauseGameObject != null) _pauseGameObject.SetActive(false);

            UpdateLanguageTxt();
            UpdateMicrophoneTxt();

            SetupVolumeSlider();

            InitTweakingOptions();

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
            if (!pressed ||_isPauseMenu) return;
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

        public void BackToMainMenu()
        {
            ReturnToGame();
            SceneManager.LoadScene("MainMenu");
        }
        #endregion

        #region Tweaking Options
        #region Init Tweaking Options

        private void InitTweakingOptions()
        {
            InitMaxShips();
            InitScaleCheckpoints();
            InitShipsMaxSpeed();
            InitShipsMinSpeed();
            InitMonsterMaxSpeed();
            InitMaxTimeSpawnShips();
            InitMinTimeSpawnShips();
            InitGodPointsFactor();
            InitHumansPointsFactor();
        }

        public void ResetTweakingOptions()
        {
            InitMaxShips(true);
            InitScaleCheckpoints(true);
            InitShipsMaxSpeed(true);
            InitShipsMinSpeed(true);
            InitMonsterMaxSpeed(true);
            InitMaxTimeSpawnShips(true);
            InitMinTimeSpawnShips(true);
            InitGodPointsFactor(true);
            InitHumansPointsFactor(true);
        }

        private void InitMaxShips(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._maxShipOptionKey))
            {
                SetMaxShips(_shipsManagerData.MaxShips);
            }

            _maxShipNumberSlider.value = PlayerPrefs.GetInt(TweakableOptionsManager._maxShipOptionKey);
        }

        private void InitScaleCheckpoints(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._scaleCheckpointsOptionKey))
            {
                SetScaleCheckpoints(_shipObjectivesData.CheckpointScaleMultiplier);
            }

            _checkpointScaleSlider.value = PlayerPrefs.GetFloat(TweakableOptionsManager._scaleCheckpointsOptionKey);
        }

        private void InitShipsMaxSpeed(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._shipsMaxSpeedOptionKey))
            {
                SetShipsMaxSpeed(_shipData.MaxSpeed);
            }

            _maxSpeedShipsSlider.value = PlayerPrefs.GetFloat(TweakableOptionsManager._shipsMaxSpeedOptionKey);
        }

        private void InitShipsMinSpeed(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._shipsMinSpeedOptionKey))
            {
                SetShipsMinSpeed(_shipData.MinSpeed);
            }

            _minSpeedShipsSlider.value = PlayerPrefs.GetFloat(TweakableOptionsManager._shipsMinSpeedOptionKey);
        }

        private void InitMonsterMaxSpeed(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._monsterMaxSpeedOptionKey))
            {
                SetMonsterMaxSpeed(_monsterData.MoveSpeed);
            }

            _maxSpeedMonsterSlider.value = PlayerPrefs.GetFloat(TweakableOptionsManager._monsterMaxSpeedOptionKey);
        }

        private void InitMaxTimeSpawnShips(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._maxTimeSpawnShipsOptionKey))
            {
                SetMaxTimeSpawnShips(_shipsManagerData.SpawnRandomMaxRate);
            }

            _maxTimeBeforeShipSpawnSlider.value = PlayerPrefs.GetFloat(TweakableOptionsManager._maxTimeSpawnShipsOptionKey);
        }

        private void InitMinTimeSpawnShips(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._minTimeSpawnShipsOptionKey))
            {
                SetMinTimeSpawnShips(_shipsManagerData.SpawnRandomMinRate);
            }

            _minTimeBeforeShipSpawnSlider.value = PlayerPrefs.GetFloat(TweakableOptionsManager._minTimeSpawnShipsOptionKey);
        }

        private void InitGodPointsFactor(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._godPointsFactorOptionKey))
            {
                SetGodPointsFactor(_victoryData.GodPersonalMultiplier);
            }

            _godPointsFactorSlider.value = PlayerPrefs.GetFloat(TweakableOptionsManager._godPointsFactorOptionKey);
        }

        private void InitHumansPointsFactor(bool reset = false)
        {
            if (reset || !PlayerPrefs.HasKey(TweakableOptionsManager._humansPointsFactorOptionKey))
            {
                SetHumansPointsFactor(_victoryData.HumanPersonalMultiplier);
            }

            _humansPointsFactorSlider.value = PlayerPrefs.GetFloat(TweakableOptionsManager._humansPointsFactorOptionKey);
        }

        #endregion

        #region Set Tweaking Options
        public void SetMaxShips(float value)
        {
            TweakableOptionsManager.Instance.SetMaxShips(value);
        }

        public void SetScaleCheckpoints(float value)
        {
            TweakableOptionsManager.Instance.SetScaleCheckpoints(value);
        }

        public void SetShipsMaxSpeed(float value)
        {
            TweakableOptionsManager.Instance.SetShipsMaxSpeed(value);
        }

        public void SetShipsMinSpeed(float value)
        {
            TweakableOptionsManager.Instance.SetShipsMinSpeed(value);
        }

        public void SetMonsterMaxSpeed(float value)
        {
            TweakableOptionsManager.Instance.SetMonsterMaxSpeed(value);
        }

        public void SetMaxTimeSpawnShips(float value)
        {
            TweakableOptionsManager.Instance.SetMaxTimeSpawnShips(value);
        }

        public void SetMinTimeSpawnShips(float value)
        {
            TweakableOptionsManager.Instance.SetMinTimeSpawnShips(value);
        }

        public void SetGodPointsFactor(float value)
        {
            TweakableOptionsManager.Instance.SetGodPointsFactor(value);
        }

        public void SetHumansPointsFactor(float value)
        {
            TweakableOptionsManager.Instance.SetHumansPointsFactor(value);
        }

        #endregion

        #endregion
    }
}
