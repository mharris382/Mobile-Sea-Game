using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InteractionTrigger = Player.InteractionTrigger;
using IInteractionTrigger=Player.IInteractionTrigger;
using UnityEngine;
using Utilities;
namespace Holdables
{
    public class HoldableProvider 
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
            return _trigger.GetInRangeInteractables<IHoldable>().FirstOrDefault(t => !t.IsHeld && _holder.HeldObject != t);
            //     .OrderBy(t =>
            // {
            //     var dist = Vector2.Distance(_transform.position, t.rigidbody2D.transform.position);
            //     if (_recentlyDropped.Contains((IHoldable)t)) dist = -(1 / dist);
            //     return dist;
            // } )

        }

        IHoldable[] GetInRangeHoldables()
        {
            return _trigger.GetInRangeInteractables<IHoldable>().ToArray();


            IEnumerable<HoldableObject> holdableObjects =
                from holdable in _trigger.GetInRangeInteractables<HoldableObject>()
                where !holdable.IsHeld select holdable;
            return holdableObjects.ToArray();
        }
    }
    
}