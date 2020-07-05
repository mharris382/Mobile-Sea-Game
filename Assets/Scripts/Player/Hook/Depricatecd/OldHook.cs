using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class OldHook : MonoBehaviour, IHoldable, IHolder
    {
        public Holder hookHolder;
        public InteractionTrigger diverInteractTrigger;
        private Rigidbody2D _rigidbody2D;
        public float maxLength = 1;

        public Rigidbody2D rigidbody2D
        {
            get => _rigidbody2D;
            set => _rigidbody2D = value;
        }

        public bool listenToInput;

        public event Action OnHookPickedUp;
        public event Action<IHoldable> OnObjectHooked;
        public event Action OnHookReleased;


        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public bool isBeingHeld;

        #region [IHoldable Implementation]

        IHoldable IHolder.HeldObject
        {
            get => hookHolder.HeldObject;
            set => hookHolder.HeldObject = value;
        }

        public bool IsHoldingObject => hookHolder.IsHoldingObject;

        bool IHolder.TryHoldObject(IHoldable objectToHold, Holder.JointHolderBase jointHolder)
        {
            return hookHolder.TryHoldObject(objectToHold, jointHolder);
        }


        public void ReleaseObject()
        {
            hookHolder.ReleaseObject();
        }

        #endregion

        public bool CanBePickedUpBy(Holder holder)
        {
            return !isBeingHeld && holder != hookHolder;
        }

        public void OnPickedUp(Holder holder)
        {
            if (this.hookHolder.IsHoldingObject)
                this.hookHolder.ReleaseObject();
            OnHookPickedUp?.Invoke();
            isBeingHeld = true;
        }

        public void OnReleased()
        {
            isBeingHeld = false;
            OnHookReleased?.Invoke();
            
            IHoldable[] inRangeHoldables = diverInteractTrigger.GetInRangeInteractables<IHoldable>()
                .Where(t => t != this && t.CanBePickedUpBy(hookHolder)).ToArray();
            var toPickup = inRangeHoldables.FirstOrDefault(t => hookHolder.TryHoldObject(t, new Holder.TargetJointHolder(t.rigidbody2D, this.transform.position )));
            if (toPickup == null)
                return;
            
            OnObjectHooked?.Invoke(toPickup);
            
        }


        public void TryToAttachHook(IHoldable[] validHoldables)
        {
            var inRangeHoldables = (from interactable in validHoldables
                where (interactable is IHoldable && ((IHoldable) interactable).CanBePickedUpBy(hookHolder))
                select interactable as IHoldable).ToArray();

            if (inRangeHoldables == null || inRangeHoldables.Length == 0)
            {
                return;
            }

            HookObject(inRangeHoldables.FirstOrDefault());
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (isBeingHeld)
            {
                var inRangeInteractables = diverInteractTrigger.GetInRangeInteractables();
                var inRangeHoldables = (from interactable in inRangeInteractables
                    where (interactable is IHoldable && ((IHoldable) interactable).CanBePickedUpBy(hookHolder))
                    select interactable as IHoldable).ToArray();

                if (inRangeHoldables == null || inRangeHoldables.Length == 0)
                {
                    return;
                }

                HookObject(inRangeHoldables.FirstOrDefault());
            }
        }

        private void HookObject(IHoldable holdable)
        {
            if (!hookHolder.TryHoldObject(holdable,
                new Holder.TargetJointHolder(holdable.rigidbody2D, transform.position, maxLength)))
            {
                throw new Exception("joint failed to pickup the selected holdable!");
            }

            OnObjectHooked?.Invoke(holdable);
        }
    }
}