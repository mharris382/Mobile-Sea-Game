using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Hook : MonoBehaviour, IHoldable
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
        
        public event Action<IHoldable> OnObjectHooked;
        public event Action OnHookReleased;
        
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public bool isBeingHeld;
        

        public bool CanBePickedUpBy(Holder holder)
        {
            return !isBeingHeld && holder != hookHolder;
        }

        public void OnPickedUp(Holder holder)
        {
            if (this.hookHolder.IsHoldingObject)
                this.hookHolder.ReleaseObject();

            isBeingHeld = true;
        }

        public void OnReleased()
        {
            isBeingHeld = false;
            OnHookReleased?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (isBeingHeld)
            {
                var inRangeInteractables = diverInteractTrigger.GetInRangeInteractables();
                var inRangeHoldables =( from interactable in inRangeInteractables
                    where (interactable is IHoldable &&((IHoldable) interactable).CanBePickedUpBy(hookHolder) )
                    select interactable as IHoldable).ToArray();

                if (inRangeHoldables == null || inRangeHoldables.Length == 0)
                {
                    return;
                }

                HookObject( inRangeHoldables.FirstOrDefault());
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