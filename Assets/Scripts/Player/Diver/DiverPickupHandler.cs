using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Diver
{
   
    public class DiverPickupHandler : MonoBehaviour
    {
        public DiverSmoothMovement diverMovement;
        public Transform fixedAttachPoint;
        
        public Holder holder;

        
        private bool isDiverHoldingHook => holder.IsHoldingObject && holder.HeldObject.rigidbody2D.CompareTag("Hook");
        private HashSet<IHoldable> _recentlyDroppedHoldables = new HashSet<IHoldable>();
        
        //TODO: Replace this method with factory pattern 
        private Holder.JointHolderBase GetHoldJoint(Rigidbody2D holdable)
        {
            float distance = Vector2.Distance(diverMovement.rigidbody2D.position, holdable.position);
            if (holdable.CompareTag("Hook"))
            {
                return new Holder.FixedJointHolder(holdable, diverMovement.rigidbody2D, fixedAttachPoint, distance);
            }

            Holder.JointHolderBase holdJoint = !holdable.isKinematic
                ? (Holder.JointHolderBase)
                new Holder.SpringJointHolder(holdable, diverMovement.rigidbody2D, distance)
                : new Holder.FixedJointHolder(holdable, diverMovement.rigidbody2D, fixedAttachPoint, distance);

            return holdJoint;
        }

        public void OnInteract(IHoldable[] validHoldables)
        {
            //if holding the hook, pass the valid holdables to the hook as well
            
            if (holder.IsHoldingObject)
            {
                DropHeldObjects();
                return;
            }
            
            var holdable = validHoldables
                .Where(t => !_recentlyDroppedHoldables.Contains(t))
                .FirstOrDefault(t => holder.TryHoldObject(t, GetHoldJoint(t.rigidbody2D)));
            
            
            if (holdable == null)
            {
                Debug.Log("Failed to pickup any holdables!");
                return;
            }
            Debug.Assert(holder.IsHoldingObject, "WTF");
            //if hook is holding an object and we picked up hook, make sure hook releases the object
            IHolder h = holdable.rigidbody2D.GetComponent<IHolder>();
            if (h != null && h.IsHoldingObject)
            {
                h.ReleaseObject();
            }
        }

        public void DropHeldObjects()
        {
            var dropped = holder.HeldObject;
            holder.ReleaseObject();
            StartCoroutine(ResetPickupDelay(dropped));
        }

        IEnumerator ResetPickupDelay(IHoldable holdable)
        {
            _recentlyDroppedHoldables.Add(holdable);
            yield return new WaitForSeconds(1.25f);
            _recentlyDroppedHoldables.Remove(holdable);
        }
    }
}