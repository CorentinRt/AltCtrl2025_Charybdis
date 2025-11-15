using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class UI_PostGame : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private GameObject _mainAnchor;

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

        private void OnDestroy()
        {
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
                    SetEnablePostGame(false);
                    break;
                case GameManager.GAME_PHASES.IN_GAME:
                    SetEnablePostGame(false);
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

    }
}
