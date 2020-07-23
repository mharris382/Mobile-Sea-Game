using System;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Zenject;

namespace Holdables
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PickupHandler : MonoBehaviour
    {
        [SerializeField] private Transform attachPoint;
        private Rigidbody2D rb;
        private HoldableProvider _pickupProvider;
        private IHolder holder;
        private IInteractionTrigger it;
        private float _timeLastInputTriggered;


        [Inject]
        private void Install(Holder holder, Rigidbody2D rb)
        {
            this.holder = holder;
            this.rb = rb;
        }


        private void Start()
        {
            if (attachPoint == null)
            {
                attachPoint = new GameObject().transform;
                attachPoint.parent = transform;
                attachPoint.localPosition = Vector3.zero;
            }

            this.it = GetComponentInChildren<InteractionTrigger>();
            this._pickupProvider = new HoldableProvider(holder, it, attachPoint);

            //TODO: move these callbacks into handler class which will probably get depricated
            SubscribeCallbacks();
            void SubscribeCallbacks()
            {
                bool wasKinematicOnPickup = false;

                //handle dropping
                holder.OnReleased += holdable =>
                {
                    //will replace with joint code
                    holdable.rigidbody2D.isKinematic = wasKinematicOnPickup;
                    holdable.rigidbody2D.transform.parent = null;
                };

                //handle pickup
                holder.OnPickedUp += holdable =>
                {
                    wasKinematicOnPickup = holdable.rigidbody2D.isKinematic;
                    //will replace with joint code
                    holdable.rigidbody2D.isKinematic = true;
                    holdable.rigidbody2D.transform.parent = attachPoint;
                    holdable.rigidbody2D.transform.localPosition = Vector3.zero;
                };
            }
        }

        //called by unity event in PlayerInput1
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (((ButtonControl) context.control).wasPressedThisFrame)
            {
                if ((Time.realtimeSinceStartup - _timeLastInputTriggered) < 0.25f)
                    return;
                _timeLastInputTriggered = Time.realtimeSinceStartup;
                Debug.Log("btn is true");
                if (holder.HeldObject == null)
                {
                    TryToPickup();
                }
                else
                {
                    ReleaseHeldObject();
                }
            }
        }

        private void TryToPickup()
        {
            IHoldable pickedUp = _pickupProvider.GetFirstChoiceForPickup();
            if (pickedUp != null)
            {
                PickUp(pickedUp);
            }
        }

        private void PickUp(IHoldable holdable)
        {
            this.holder.PickupObject(holdable);
        }


        private void ReleaseHeldObject()
        {
            holder.ReleaseHeldObject();
        }
    }
}