using Diver;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Holdables.Diver
{
    [TypeInfoBox("Listens for input and triggers pickups and drops")]
    public class DiverPickupHandler : MonoBehaviour
    {
        private DiverInput _input;
        private Holder _holder;
        private HoldableProvider _provider;
        
        
        [Inject]
        void Inject(Holder diverHolder, HoldableProvider holdableProvider, DiverInput diverInput)
        {
            this._holder = diverHolder;
            this._provider = holdableProvider;
            this._input = diverInput;
        }

        private void Start()
        {
            // _input.OnInteract.Where(t => _holder.HeldObject == null)
            //     .Select(t => _provider.GetFirstChoiceForPickup()).
            //     Where(t => t != null).
            //     Subscribe(t => _holder.PickupObject((t)));
            //
            // _input.OnInteract.Where(t => _holder.HeldObject != null)
            //     .Subscribe(_ => _holder.ReleaseHeldObject());
            
            _input.OnInteract.Subscribe(_ =>
            {
                if (_holder.HeldObject != null)
                {
                    _holder.ReleaseHeldObject();
                }
                else
                {
                    var holdable = _provider.GetFirstChoiceForPickup();
                    if(holdable != null)_holder.PickupObject(holdable);
                }
            });
        }

        
    }
}