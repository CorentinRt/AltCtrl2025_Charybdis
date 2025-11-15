using UnityEngine;
using UnityEngine.UI;

namespace AltCtrl.Charybdis
{
    public class UI_Victory : MonoBehaviour
    {
        #region Fields
        [Header("Anchor")]
        [SerializeField] private GameObject _mainAnchor;

        [Header("Cont")]
        [SerializeField] private GameObject _mainContainerVictory;
        [SerializeField] private GameObject _humanContainerVictory;
        [SerializeField] private GameObject _godContainerVictory;

        [Header("Slider")]
        [SerializeField] private Slider _slider;
        [SerializeField] private GameObject _movingFill;
        [SerializeField] private Transform _fillMin;
        [SerializeField] private Transform _fillMax;
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
            _mainContainerVictory.SetActive(false);
            _humanContainerVictory.SetActive(false);
            _godContainerVictory.SetActive(false);

            if (VictoryManager.Exist)
            {
                VictoryManager.Instance.OnGodVictory += ReactOnGodVictory;
                VictoryManager.Instance.OnHumanVictory += ReactOnHumanVictory;
            }

            _slider.onValueChanged.AddListener(UpdateSliderFillPosition);
        }

        private void OnDestroy()
        {
            if (VictoryManager.Exist)
            {
                VictoryManager.Instance.OnGodVictory -= ReactOnGodVictory;
                VictoryManager.Instance.OnHumanVictory -= ReactOnHumanVictory;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePhaseChanged -= ReactOnGamePhaseChanged;
            }

            _slider.onValueChanged.RemoveListener(UpdateSliderFillPosition);
        }

        private void ReactOnGamePhaseChanged(GameManager.GAME_PHASES gamePhase)
        {
            switch (gamePhase)
            {
                case GameManager.GAME_PHASES.PRE_GAME:
                    SetEnablePostGame(false);
                    break;
                case GameManager.GAME_PHASES.IN_GAME:
                    SetEnablePostGame(true);
                    break;
                case GameManager.GAME_PHASES.POST_GAME:
                    SetEnablePostGame(true);
                    break;
                case GameManager.GAME_PHASES.TUTO_SHOW:
                    SetEnablePostGame(false);
                    break;
            }
        }

        private void SetEnablePostGame(bool enabled)
        {
            _mainAnchor.SetActive(enabled);
        }

        private void ReactOnHumanVictory()
        {
            _humanContainerVictory.SetActive(true);

            ReactOnGeneralVictory();
        }

        private void ReactOnGodVictory()
        {
            _godContainerVictory.SetActive(true);

            ReactOnGeneralVictory();
        }

        private void ReactOnGeneralVictory()
        {
            _mainContainerVictory.SetActive(true);
        }

        private void UpdateSliderFillPosition(float value)
        {
            float percentage = value / _slider.maxValue;

            Vector2 newPosition = Vector2.Lerp(_fillMin.position, _fillMax.position, percentage);

            _movingFill.transform.position = newPosition;   
        }
    }
}
