using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player.Diver
{
    public class DiverPickupHandler
    {
        private readonly Holder _holder;
        private readonly DiverJointHolderFactory _jointFactory;
        private readonly HashSet<IHoldable> _recentlyDropped;


        public DiverPickupHandler(Holder holder, DiverJointHolderFactory jointFactory)
        {
            _holder = holder;
            _jointFactory = jointFactory;
            _recentlyDropped = new HashSet<IHoldable>();
        }

        public void OnInteract(IEnumerable<IHoldable> validHoldables)
        {
            //if holding the hook, pass the valid holdables to the hook as well

            if (_holder.IsHoldingObject)
            {
                DropHeldObjects();
                return;
            }


            if (validHoldables == null || validHoldables.FirstOrDefault() == null)
                return;

            IHoldable holdable = validHoldables
                .OrderBy(t => _recentlyDropped.Contains(t) ? 1 : -1)
                .FirstOrDefault(t => t.CanBePickedUpBy(_holder));

            if (holdable != null && _holder.TryHoldObject(holdable, _jointFactory.Create(holdable.rigidbody2D)))
            {
                //_isHoldingObject = true;
                return;
            }

            // var holdable = validHoldables
            //     .Where(t => !_recentlyDroppedHoldables.Contains(t))
            //     .FirstOrDefault(t => holder.TryHoldObject(t, GetHoldJoint(t.rigidbody2D)));
            //
            //
            // if (holdable == null)
            // {
            //     Debug.Log("Failed to pickup any holdables!");
            //     return;
            // }
            // Debug.Assert(holder.IsHoldingObject, "WTF");
            // //if hook is holding an object and we picked up hook, make sure hook releases the object
            // IHolder h = holdable.rigidbody2D.GetComponent<IHolder>();
            // if (h != null && h.IsHoldingObject)
            // {
            //     h.ReleaseObject();
            // }
        }

        public void DropHeldObjects()
        {
            var dropped = _holder.HeldObject;
            _holder.ReleaseObject();
            CoroutineHandler.instance.StartCoroutine(ResetPickupDelay(dropped));
        }

        IEnumerator ResetPickupDelay(IHoldable holdable)
        {
            _recentlyDropped.Add(holdable);
            yield return new WaitForSeconds(1.25f);
            _recentlyDropped.Remove(holdable);
        }

        // private Holder.JointHolderBase GetHoldJoint(Rigidbody2D holdable)
        // {
        //     float distance = Vector2.Distance(diverMovement.position, holdable.position);
        //     if (holdable.CompareTag("Hook"))
        //     {
        //         return new Holder.FixedJointHolder(holdable, diverMovement, fixedAttachPoint, distance);
        //     }
        //
        //     Holder.JointHolderBase holdJoint = !holdable.isKinematic
        //         ? (Holder.JointHolderBase)
        //         new Holder.SpringJointHolder(holdable, diverMovement, distance)
        //         : new Holder.FixedJointHolder(holdable, diverMovement, fixedAttachPoint, distance);
        //
        //     return holdJoint;
        // }
        //
        //
        // private Holder.JointHolderBase _GetHoldJoint(Rigidbody2D holdable)
        // {
        //     float distance = Vector2.Distance(diverMovement.position, holdable.position);
        //     
        //     Holder.JointHolderBase holdJoint = holdable.isKinematic || holdable.CompareTag("Hook") ?  
        //         (Holder.JointHolderBase)
        //         new Holder.FixedJointHolder(holdable, diverMovement, fixedAttachPoint, distance):
        //         new Holder.SpringJointHolder(holdable, diverMovement, distance);
        //
        //     return holdJoint;
        // }
    }
}