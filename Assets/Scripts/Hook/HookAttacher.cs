using System;
using Holdables;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using InteractionTrigger= Player.InteractionTrigger;
using Holdables;

namespace Hook
{
    public class HookAttacher : MonoBehaviour
    {
        private Holder _holder;
        private IHoldableProvider _holdableProvider;
        private float _timeLastInputTriggered;
        private SpringJoint2D _springJoint2D;
        private Rigidbody2D _rb;

        [Inject]
        void Install(Rigidbody2D rb, Holder holder)
        {
            _holder = holder;
            _holdableProvider = new HookHoldableProvider(rb, holder, GetComponentInChildren<InteractionTrigger>());
            _rb = rb;
        }

        public void OnHookAttached(HookHeldItemChangedSignal attachSignal)
        {
            if (attachSignal.HeldObject == null)
            {
                _springJoint2D.connectedBody = null;
                _springJoint2D.enabled = false;
            }
            else if(attachSignal.HeldObject.rigidbody2D != null)
            {
                _springJoint2D.connectedBody = attachSignal.HeldObject.rigidbody2D;
                _springJoint2D.enabled = true;
            }
        }

        private void Awake()
        {
            _springJoint2D = GetComponent<SpringJoint2D>();
            _springJoint2D.enabled = false;
        }


        public void OnHookAttachButton(InputAction.CallbackContext context)
        {
            if ((Time.realtimeSinceStartup - _timeLastInputTriggered) < 0.25f)
                return;
            _timeLastInputTriggered = Time.realtimeSinceStartup;
            Debug.Log("OnHookAttachedButton Pressed".InItalics());
            if (_holder.HeldObject != null)
            {
                _holder.ReleaseHeldObject();
            }
            else
            {
                IHoldable best= _holdableProvider.GetFirstChoiceForPickup();
                if (best != null)
                {
                    _holder.PickupObject(best);
                }
            }
        }

    }
}