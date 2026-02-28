using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class TutoController_Human : TutoController
    {
        #region Fields
        [Header("Data")]
        [SerializeField] private SO_HumanTutoController_Data _data;

        [Header("UI Radio")]
        [SerializeField] private UI_Radio _uiRadio;

        private bool _hasRightFrequency = false;

        private bool _hasValidatedOneShip = false;

        private Coroutine _tutoHumanCoroutine;

        #endregion

        #region Properties


        #endregion

        protected override void Start()
        {
            ShipsManager.Instance.SetEnableShipsSpawn(false, true);

            ShipsManager.Instance.OnDestroyShip += ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShipWithoutObjective += ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShip += ReceiveOnValidateShip;
            ShipsManager.Instance.OnControlNewShipWithRadio += ReceiveOnControlNewShipWithRadio;

            VictoryManager.Instance.OnHumanVictory += ReceiveOnHumanVictory;

            VictoryManager.Instance.PreventHumanVictory = true;

            RadioManager.Instance.EnableRadioOnPause(true);
            RadioManager.Instance.SetEnableRadioInput(false, true);

            base.Start();
        }

        protected override void OnDestroy()
        {
            ShipsManager.Instance.OnDestroyShip -= ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShipWithoutObjective -= ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShip -= ReceiveOnValidateShip;
            ShipsManager.Instance.OnControlNewShipWithRadio -= ReceiveOnControlNewShipWithRadio;

            VictoryManager.Instance.OnHumanVictory -= ReceiveOnHumanVictory;

            base.OnDestroy();
        }

        protected override void StartTuto()
        {
            base.StartTuto();

            if (_tutoHumanCoroutine != null)
            {
                StopCoroutine(_tutoHumanCoroutine);
                _tutoHumanCoroutine = null;
            }

            _tutoHumanCoroutine = StartCoroutine(TutoHumanCoroutine());

            InputManager.Instance.SetDetectGodInput(false);
            InputManager.Instance.SetDetectHumanInput(true);
        }

        private void ReceiveOnDestroyShip()
        {
            if (!_hasValidatedOneShip)
            {
                ShipsManager.Instance.SpawnShip();
            }
        }

        private void ReceiveOnValidateShip()
        {
            _hasValidatedOneShip = true;
        }

        private void ReceiveOnHumanVictory()
        {
            InputManager.Instance.SetDetectGodInput(true);
            InputManager.Instance.SetDetectHumanInput(true);
            VictoryManager.Instance.OnHumanVictory -= ReceiveOnHumanVictory;
            ShipsManager.Instance.SetEnableShipsSpawn(false, true);
        }

        private void ReceiveOnControlNewShipWithRadio()
        {
            _hasRightFrequency = true;
        }


        private IEnumerator TutoHumanCoroutine()
        {
            // désactiver iles
            SetEnabledAllIslands(false);

            yield return new WaitForSeconds(_data.TimeBeforeSpawnFirstShip);

            // apparition bateau avec frequence
            ShipsManager.Instance.SpawnShip();

            yield return new WaitForSeconds(_data.TimeBeforeFrequencyTutoAppear);

            // apparition tuto fréquence
            SetEnabledTutorialUI(true);
            GoToTutorialUIPart("Frequency");

            yield return new WaitForSeconds(_data.TimeBeforeFreezeTime);

            _uiRadio.SetEnableGamePhase(true);
            RadioManager.Instance.SetEnableRadioInput(true, true);

            while (!_hasRightFrequency)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, 0.9f);

                yield return null;
            }

            Time.timeScale = 1f;

            ShipsManager.Instance.OnControlNewShipWithRadio -= ReceiveOnControlNewShipWithRadio;

            while (!_hasValidatedOneShip)
            {

                yield return null;
            }

            ShipsManager.Instance.OnDestroyShip -= ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShipWithoutObjective -= ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShip -= ReceiveOnValidateShip;

            GoToTutorialUIPart("Fade");
            SetEnabledTutorialUI(false);

            yield return new WaitForSeconds(_data.TimeBeforeDangerTutoAndIslandsAppear);

            SetEnabledAllIslands(true);
            SetEnabledTutorialUI(true);
            GoToTutorialUIPart("Dangers");

            yield return new WaitForSeconds(_data.TimeBeforeDemoGameStart);

            GoToTutorialUIPart("Fade");
            SetEnabledTutorialUI(false);

            ShipsManager.Instance.SpawnShip();

            ShipsManager.Instance.SetEnableShipsSpawn(true, true);

            VictoryManager.Instance.PreventHumanVictory = false;

            yield return null;
        }
    }
}
