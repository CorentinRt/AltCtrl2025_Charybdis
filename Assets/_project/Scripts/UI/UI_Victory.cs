using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class UI_Victory : MonoBehaviour
    {
        #region Fields
        [Header("Cont")]
        [SerializeField] private GameObject _mainContainerVictory;
        [SerializeField] private GameObject _humanContainerVictory;
        [SerializeField] private GameObject _godContainerVictory;

        #endregion

        #region Properties


        #endregion

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
        }

        private void OnDestroy()
        {
            if (VictoryManager.Exist)
            {
                VictoryManager.Instance.OnGodVictory -= ReactOnGodVictory;
                VictoryManager.Instance.OnHumanVictory -= ReactOnHumanVictory;
            }
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

    }
}
