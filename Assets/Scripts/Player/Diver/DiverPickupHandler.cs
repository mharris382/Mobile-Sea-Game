﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Diver
{
   
    public class DiverPickupHandler : MonoBehaviour
    {
        public Rigidbody2D diverMovement;
        public Transform fixedAttachPoint;
        
        public Holder holder;

        
        private bool isDiverHoldingHook => holder.IsHoldingObject && holder.HeldObject.rigidbody2D.CompareTag("Hook");
        private HashSet<IHoldable> _recentlyDroppedHoldables = new HashSet<IHoldable>();

        private DiverJointHolderFactory _jointFactory;


        private void Awake()
        {
            _jointFactory = new DiverJointHolderFactory(diverMovement, fixedAttachPoint);
        }

        public void OnInteract(IHoldable[] validHoldables)
        {
            //if holding the hook, pass the valid holdables to the hook as well
            
            if (holder.IsHoldingObject)
            {
                DropHeldObjects();
                return;
            }
            
            IHoldable holdable = validHoldables
                .OrderBy(t => _recentlyDroppedHoldables.Contains(t) ? 1 : -1)
                .FirstOrDefault(t => t.CanBePickedUpBy(holder));
            
            if (holdable != null  && holder.TryHoldObject(holdable,  _GetHoldJoint(holdable.rigidbody2D)))
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

        private Holder.JointHolderBase GetHoldJoint(Rigidbody2D holdable)
        {
            float distance = Vector2.Distance(diverMovement.position, holdable.position);
            if (holdable.CompareTag("Hook"))
            {
                return new Holder.FixedJointHolder(holdable, diverMovement, fixedAttachPoint, distance);
            }

            Holder.JointHolderBase holdJoint = !holdable.isKinematic
                ? (Holder.JointHolderBase)
                new Holder.SpringJointHolder(holdable, diverMovement, distance)
                : new Holder.FixedJointHolder(holdable, diverMovement, fixedAttachPoint, distance);

            return holdJoint;
        }


        private Holder.JointHolderBase _GetHoldJoint(Rigidbody2D holdable)
        {
            float distance = Vector2.Distance(diverMovement.position, holdable.position);
            
            Holder.JointHolderBase holdJoint = holdable.isKinematic || holdable.CompareTag("Hook") ?  
                (Holder.JointHolderBase)
                new Holder.FixedJointHolder(holdable, diverMovement, fixedAttachPoint, distance):
                new Holder.SpringJointHolder(holdable, diverMovement, distance);

            return holdJoint;
        }
    }


    public class DiverJointHolderFactory : IFactory<Holder.JointHolderBase, Rigidbody2D>
    {
        private Rigidbody2D diverMovement;
        private readonly Transform _attachPoint;

        public DiverJointHolderFactory(Rigidbody2D diverMovement, Transform attachPoint)
        {
            this.diverMovement = diverMovement;
            _attachPoint = attachPoint;
        }

        public Holder.JointHolderBase Create(Rigidbody2D holdable)
        {
            float distance = Vector2.Distance(diverMovement.position, holdable.position);
            
            if (holdable.isKinematic || holdable.CompareTag("Hook"))
            {
                return new Holder.FixedJointHolder(holdable, diverMovement, _attachPoint, distance/2f );
            }

            return new Holder.SpringJointHolder(holdable, diverMovement, distance);
        }
    }
    public interface IFactory<out T,in T1>
    {
        T Create(T1 arg1);
    }
}