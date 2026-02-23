using System.Collections;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class TutoController_God : TutoController
    {
        #region Fields



        private bool _hasDestroyedOneShip = false;

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

            base.Start();
        }

        protected override void OnDestroy()
        {
            ShipsManager.Instance.OnDestroyShip -= ReceiveOnDestroyShip;
            ShipsManager.Instance.OnValidateShip -= ReceiveOnValidateShip;

            VictoryManager.Instance.OnGodVictory -= ReceiveOnGodVictory;

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

        private void ReceiveOnGodVictory()
        {
            InputManager.Instance.SetDetectGodInput(true);
            InputManager.Instance.SetDetectHumanInput(true);
            VictoryManager.Instance.OnGodVictory -= ReceiveOnGodVictory;
            ShipsManager.Instance.SetEnableShipsSpawn(false, true);
        }

        private IEnumerator TutoGodCoroutine()
        {
            SetEnabledAllIslands(false);

            // wait cooldown
            yield return new WaitForSeconds(3f);

            // show tuto UI
            SetEnabledTutorialUI(true);

            yield return new WaitForSeconds(10f);

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
            yield return new WaitForSeconds(1f);

            /*
            // Faire apparaitre tuto biere
            SetEnabledTutorialUI(true);

            yield return new WaitForSeconds(10f);

            SetEnabledTutorialUI(false);
            */

            // spawn iles
            SetEnabledAllIslands(true);

            yield return new WaitForSeconds(1f);

            ShipsManager.Instance.SetEnableShipsSpawn(true, true);

            yield return new WaitForSeconds(3f);

            VictoryManager.Instance.PreventGodVictory = false;

            yield return null;
        }

    }
}
