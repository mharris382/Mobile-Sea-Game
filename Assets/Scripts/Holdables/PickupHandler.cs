using System;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Holdables
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PickupHandler : MonoBehaviour
    {
        [SerializeField]
        private Transform attachPoint;
        private Rigidbody2D rb;
        private HoldableProvider pickupSelector;
        private IHolder holder;
        private IInteractionTrigger it;
        private float _timeLastInputTriggered;


        private void Awake()
        {
            if (attachPoint == null)
            {
                attachPoint = new GameObject().transform;
                attachPoint.parent = transform;
                attachPoint.localPosition = Vector3.zero;
            }

            this.it = GetComponentInChildren<InteractionTrigger>();
            this.holder = new Holder(this.rb = GetComponent<Rigidbody2D>());
            this.pickupSelector = new HoldableProvider(holder, it, attachPoint);
        }

        private void Start()
        {
            //handle dropping
            holder.OnReleased += holdable =>
            {
                //will replace with joint code
                holdable.rigidbody2D.isKinematic = false;
                holdable.rigidbody2D.transform.parent = null;
            };

            //handle pickup
            holder.OnPickedUp += holdable =>
            {
                //will replace with joint code
                holdable.rigidbody2D.isKinematic = true;
                holdable.rigidbody2D.transform.parent = attachPoint;
                holdable.rigidbody2D.transform.localPosition = Vector3.zero;
            };
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
            IHoldable pickedUp = pickupSelector.GetFirstChoiceForPickup();
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