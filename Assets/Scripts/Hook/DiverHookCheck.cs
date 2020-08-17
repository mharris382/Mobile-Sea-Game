using System.Linq;
using Diver;
using Hook.Signals;
using Player;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;
using Holder = Holdables.Holder;
using IHoldable = Holdables.IHoldable;

namespace Hook
{
    [TypeInfoBox("This is responsible for checking if the diver can attach the hook to a holdable.")]
    public class DiverHookCheck : MonoBehaviour
    {
        private Holder _diverHolder;
        private InteractionTrigger _interactTrigger;
        private DiverInput _input;

        [Inject]
        void Inject(Holder holder, DiverInput input)
        {
            this._diverHolder = holder;
        
            this._input = input;
        }

        private void Awake()
        {
            _interactTrigger = GetComponentInChildren<InteractionTrigger>();
        }

        private void Start()
        {
            _input.OnUseHook.Subscribe(_ =>
            {
                if (HasAvailableHookAttachment(out var hookHolder, out var availablePickup))
                {
                    _diverHolder.ReleaseHeldObject();
                    hookHolder.Holder.PickupObject(availablePickup);
                }
            
                // hookHolder = _interactTrigger.GetInRangeInteractables<HookHolder>().FirstOrDefault();
                // if (hookHolder != null && hookHolder.Holder.HeldObject == null)
                // {
                //     Debug.Log("Diver is in range of hook");
                //     if (_diverHolder.HeldObject != null)
                //     {
                //         var held = _diverHolder.HeldObject;
                //         _diverHolder.ReleaseHeldObject();
                //         hookHolder.Holder.PickupObject(held);
                //     }
                // }
            });
        }

        bool HasAvailableHookAttachment(out HookHolder hookHolder,  out IHoldable availableAttachment)
        {
            availableAttachment = null;
            hookHolder = _interactTrigger.GetInRangeInteractables<HookHolder>().FirstOrDefault();
            if (hookHolder != null && hookHolder.Holder.HeldObject == null)
            {
                if (_diverHolder.HeldObject != null)
                {
                    availableAttachment = _diverHolder.HeldObject;
                    return true;
                }
            }

            return false;
        }

        private void LateUpdate()
        {
            if (HasAvailableHookAttachment(out var hookHolder, out var availablePickup))
            {
                MessageBroker.Default.Publish(new HookHasAttachmentAvailableSignal()
                {
                    hook = hookHolder,
                    holdable = availablePickup
                });
            }
        }
    
    
    }
}