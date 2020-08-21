using Holdables;

namespace Signals
{
    public struct DiverPickupAvailableSignal
    {
        public IHoldable availablePickup;

        public DiverPickupAvailableSignal(IHoldable availablePickup)
        {
            this.availablePickup = availablePickup;
        }
    }
}