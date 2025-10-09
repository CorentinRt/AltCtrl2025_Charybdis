using UnityEngine;

namespace AltCtrl.Charybdis
{
    public interface IShipBehaviour
    {
        public abstract ShipBehaviour GetShip();

        public abstract void Init();
    }

    public class ShipProxy : MonoBehaviour, IShipBehaviour
    {
        [SerializeField] private ShipBehaviour _associatedShip;

        public ShipBehaviour GetShip()
        {
            return _associatedShip;
        }

        public void Init()
        {
            _associatedShip.Init();
        }
    }
}
