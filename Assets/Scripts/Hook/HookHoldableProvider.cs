using Holdables;
using Player;
using UnityEngine;
using IHoldable = Holdables.IHoldable;

namespace Hook
{
    public class HookHoldableProvider : HoldableProvider
    {
        private readonly Rigidbody2D _hookRb;

        public HookHoldableProvider(Rigidbody2D hookRb, IHold holder, IInteractionTrigger trigger) : base(holder, trigger, hookRb.transform)
        {
            _hookRb = hookRb;
        }

        protected override bool CanPickup(IHoldable t)
        {
            return t.rigidbody2D != _hookRb && base.CanPickup(t);
        }
    }
}