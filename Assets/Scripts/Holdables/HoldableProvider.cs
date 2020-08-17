using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InteractionTrigger = Player.InteractionTrigger;
using IInteractionTrigger=Player.IInteractionTrigger;
using UnityEngine;
using Utilities;
namespace Holdables
{
  
    public interface IHoldableProvider
    {
        IHoldable GetFirstChoiceForPickup();
    }
    
    public class HoldableProvider : IHoldableProvider
    {
        private IHold _holder;
        private IInteractionTrigger _trigger;
        private Transform _transform;
        
        private HashSet<IHoldable> _recentlyDropped = new HashSet<IHoldable>();
        
        public HoldableProvider(IHold holder, IInteractionTrigger trigger, Transform transform)
        {
            _holder = holder;
            _trigger = trigger;
            _transform = transform;
            _holder.OnReleased += holdable => CoroutineHandler.instance.StartCoroutine(DoReset(holdable));
        }

        IEnumerator DoReset(IHoldable holdable)
        {
            if (_recentlyDropped.Contains(holdable)) _recentlyDropped.Remove(holdable);
            _recentlyDropped.Add(holdable);
            yield return new WaitForSeconds(0.5f);
            _recentlyDropped.Remove(holdable);
        }

        
        public IHoldable GetFirstChoiceForPickup()
        {
            return _trigger.GetInRangeInteractables<IHoldable>()
                .FirstOrDefault(t => CanPickup(t));
        }

        protected virtual bool CanPickup(IHoldable t)
        {
            return !t.IsHeld && _holder.HeldObject != t;
        }
    }
    
}