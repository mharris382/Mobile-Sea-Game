using Holdables;
using UnityEngine;

namespace Hook.Signals
{
    public struct HookHasAttachmentAvailableSignal
    {
        public MonoBehaviour hook;
        public IHoldable holdable;
    }
    
    
}

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