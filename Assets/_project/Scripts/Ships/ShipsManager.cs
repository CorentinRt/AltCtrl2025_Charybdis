using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using CREMOT.GameplayUtilities;

namespace AltCtrl.Charybdis
{
    public class ShipsManager : GenericSingleton<ShipsManager>
    {
        #region Fields


        private List<ShipBehaviour> _ships = new List<ShipBehaviour>();

        #endregion

        #region Properties


        #endregion

        public void AddShip(ShipBehaviour ship)
        {
            if (ship == null)
                return;

            _ships.Add(ship);
        }

        public void RemoveShip(ShipBehaviour ship)
        {
            if (ship == null)
                return;

            _ships.Remove(ship);
        }

        private void Update()
        {
            foreach (ShipBehaviour shipBehaviour in _ships)
            {
                if (shipBehaviour == null)
                    continue;

                shipBehaviour.PredictTrajectory();
            }
        }

    }
}
