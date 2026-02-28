using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class TutoController_God : TutoController
    {
        #region Fields

        [Header("Data")]
        [SerializeField] private SO_GodTutoController_Data _data;

        private bool _hasDestroyedOneShip = false;

        private bool _hasUsedBeer = false;

        private Coroutine _tutoGodCoroutine;

        #endregion

        #region Properties


        #endregion

        protected override void Start()
        {
            ShipsManager.Instance.SetEnableShipsSpawn(false, true);

            ShipsManager.Instance.OnDestroyShip += ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShip += ReceiveOnValidateShip;

            VictoryManager.Instance.PreventGodVictory = true;

            VictoryManager.Instance.OnGodVictory += ReceiveOnGodVictory;

            WindIndicator.Instance.OnStartWindOnBoats += ReceiveOnStartWindOnBoat;

            base.Start();
        }

        protected override void OnDestroy()
        {
            ShipsManager.Instance.OnDestroyShip -= ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShip -= ReceiveOnValidateShip;

            VictoryManager.Instance.OnGodVictory -= ReceiveOnGodVictory;

            WindIndicator.Instance.OnStartWindOnBoats -= ReceiveOnStartWindOnBoat;

            base.OnDestroy();
        }

        protected override void StartTuto()
        {
            base.StartTuto();

            if (_tutoGodCoroutine != null)
            {
                StopCoroutine(_tutoGodCoroutine);
                _tutoGodCoroutine = null;
            }

            _tutoGodCoroutine = StartCoroutine(TutoGodCoroutine());

            InputManager.Instance.SetDetectGodInput(true);
            InputManager.Instance.SetDetectHumanInput(false);
        }

        private void ReceiveOnDestroyShip()
        {
            _hasDestroyedOneShip = true;
        }

        private void ReceiveOnValidateShip()
        {
            ShipsManager.Instance.SpawnShip();
        }

        private void ReceiveOnStartWindOnBoat(Vector3 windForce)
        {
            _hasUsedBeer = true;
        }

        private void ReceiveOnGodVictory()
        {
            InputManager.Instance.SetDetectGodInput(true);
            InputManager.Instance.SetDetectHumanInput(true);
            VictoryManager.Instance.OnGodVictory -= ReceiveOnGodVictory;
            ShipsManager.Instance.SetEnableShipsSpawn(false, true);
        }

        private IEnumerator TutoGodCoroutine()
        {
            SetEnabledTutorialUI(false);
            SetEnabledAllIslands(false);

            // wait cooldown
            yield return new WaitForSeconds(_data.TimeBeforeMoveTutoAppear);

            // show tuto UI move
            SetEnabledTutorialUI(true);
            GoToTutorialUIPart("Move");

            yield return new WaitForSeconds(_data.TimeBeforeSpawnFirstShip);

            // hide tutorial ui
            GoToTutorialUIPart("Fade");
            SetEnabledTutorialUI(false);

            // Spawn bateau à détruire
            _hasDestroyedOneShip = false;
            ShipsManager.Instance.SpawnShip();

            // Attendre bateau détruit pour continuer
            while (!_hasDestroyedOneShip)
            {

                yield return null;
            }

            ShipsManager.Instance.OnDestroyShip -= ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShip -= ReceiveOnValidateShip;

            // ship destroyed wait before next phase
            yield return new WaitForSeconds(_data.TimeBeforeTutoBeerAppear);

            // Faire apparaitre tuto biere
            SetEnabledTutorialUI(true);
            GoToTutorialUIPart("Beer");

            _hasUsedBeer = false;

            // wait use beer
            while (!_hasUsedBeer)
            {

                yield return null;
            }

            WindIndicator.Instance.OnStartWindOnBoats -= ReceiveOnStartWindOnBoat;

            // Hide tutorial ui
            GoToTutorialUIPart("Fade");
            SetEnabledTutorialUI(false);

            yield return new WaitForSeconds(_data.TimeBeforeSpawnIslands);

            // spawn iles
            SetEnabledAllIslands(true);

            yield return new WaitForSeconds(_data.TimeBeforeSpawnShipsDemoGame);

            ShipsManager.Instance.SpawnShip();

            ShipsManager.Instance.SetEnableShipsSpawn(true, true);

            yield return new WaitForSeconds(3f);

            VictoryManager.Instance.PreventGodVictory = false;

            yield return null;
        }

    }
}
