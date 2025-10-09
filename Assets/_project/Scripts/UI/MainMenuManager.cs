using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace AltCtrl.Charybdis
{
    public class MainMenuManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Language")]
        public LocalizeStringEvent _languageLocalizeStringEvent;

        [Header("Microphone")]
        public LocalizeStringEvent _microphoneLocalizeStringEvent;
        // ----- FIELDS ----- //

        private void Start()
        {
            UpdateLanguageTxt();
            UpdateMicrophoneTxt();

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnNextMicrophonePressed += SetNextMicrophone;
                InputManager.Instance.OnNextLanguagePressed += SetNextLanguage;
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnNextMicrophonePressed -= SetNextMicrophone;
                InputManager.Instance.OnNextLanguagePressed -= SetNextLanguage;
            }
        }

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
    }
}
