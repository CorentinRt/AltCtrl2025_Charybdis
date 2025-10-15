using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class BlueIslandsIndicators : MonoBehaviour
    {
        #region Fields
        [Header("Indicators")]
        [SerializeField] private List<GameObject> _indicators;

        #endregion

        #region Properties


        #endregion

        private void Start()
        {
            EnableIndicators(false);

            if (GameManager.Exist)
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
                case GameManager.GAME_PHASES.POST_GAME:
                    EnableIndicators(true);
                    break;
            }
        }

        private void EnableIndicators(bool enable)
        {
            foreach (GameObject indicator in _indicators)
            {
                if (indicator == null)
                    continue;

                indicator.SetActive(enable);
            }
        }

    }
}
