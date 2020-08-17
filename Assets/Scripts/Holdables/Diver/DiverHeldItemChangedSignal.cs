using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace Holdables.Diver
{
    public class DiverHeldItemChangedSignal
    {
        public DiverHeldItemChangedSignal(IHoldable heldObject)
        {
            HeldObject = heldObject;
        }

        public IHoldable HeldObject { get; private set; }
        
        
        
        [UsedImplicitly]
        public class Publisher : IInitializable
        {
            private SignalBus _signalBus;
            private Holder _holder;

            public Publisher(SignalBus signalBus, Holder holder)
            {
                _signalBus = signalBus;
                _holder = holder;
            }

            public void Initialize()
            {
                var onPickup = _holder.OnPickupAsObservable().Select(t => new DiverHeldItemChangedSignal(t));
                var onRelease = _holder.OnReleasedAsObservable().Select(t => new DiverHeldItemChangedSignal(null));

                onPickup.Merge(onRelease).Subscribe(signal => _signalBus.Fire(signal));
                onPickup.Merge(onRelease).Subscribe(signal => MessageBroker.Default.Publish(signal));
                
                return;
                
                
                _holder.OnPickedUp += holdable => _signalBus.Fire(new DiverHeldItemChangedSignal(holdable));
                _holder.OnReleased += holdable => _signalBus.Fire(new DiverHeldItemChangedSignal(null));
                _holder.OnPickedUp += holdable => MessageBroker.Default.Publish(new DiverHeldItemChangedSignal(holdable));
                _holder.OnReleased += holdable => MessageBroker.Default.Publish(new DiverHeldItemChangedSignal(null));
            }
        }
    }
}