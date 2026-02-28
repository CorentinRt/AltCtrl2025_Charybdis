using UnityEngine;
using TMPro;

namespace AltCtrl.Charybdis
{
    public class UI_Radio : MonoBehaviour
    {
        #region Fields
        [SerializeField] private GameObject _mainAnchor;
        [SerializeField] private TextMeshProUGUI _frequencyLabel;

        #endregion

        #region Properties


        #endregion


        private void Awake()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePhaseChanged += ReactOnGamePhaseChanged;
            }
        }

        private void Start()
        {
            Init();
        }
        private void Init()
        {
            if (RadioManager.Exist)
            {
                RadioManager.Instance.OnChangeFrequency += UpdateFrequencyLabel;

                UpdateFrequencyLabel(RadioManager.Instance.GetCurrentFrequency());
            }

        }


        private void OnDestroy()
        {
            if (RadioManager.Exist)
            {
                RadioManager.Instance.OnChangeFrequency -= UpdateFrequencyLabel;
            }

            if (GameManager.Exist)
            {
                GameManager.Instance.OnGamePhaseChanged -= ReactOnGamePhaseChanged;
            }
        }

        private void ReactOnGamePhaseChanged(GameManager.GAME_PHASES gamePhase)
        {
            switch (gamePhase)
            {
                case GameManager.GAME_PHASES.PRE_GAME:
                    SetEnableGamePhase(false);
                    break;
                case GameManager.GAME_PHASES.IN_GAME:
                    SetEnableGamePhase(true);
                    break;
                case GameManager.GAME_PHASES.POST_GAME:
                    SetEnableGamePhase(false);
                    break;
                case GameManager.GAME_PHASES.TUTO_SHOW:
                    SetEnableGamePhase(false);
                    break;
            }
        }

        public void SetEnableGamePhase(bool enabled)
        {
            _mainAnchor.SetActive(enabled);
        }

        private void UpdateFrequencyLabel((int, int) frequency)
        {
            _frequencyLabel.text = $"{ frequency.Item1 }.{ frequency.Item2 }";
        }
    }
}
