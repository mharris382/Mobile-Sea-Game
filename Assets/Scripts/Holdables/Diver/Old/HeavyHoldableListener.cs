using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace Holdables.Diver
{
    [UsedImplicitly]
    public class HeavyHoldableListener : IInitializable
    {
        public ReadOnlyReactiveProperty<bool> DiverIsCarryingHeavyHoldable { get; set; }
        
        public void Initialize()
        {
            
            
            IObservable<bool> isCarryingHeavyHoldable = 
                MessageBroker.Default.Receive<DiverHeldItemChangedSignal>()
                    .Select(t => t.HeldObject != null && HeavyHoldable.heavyHoldables.Contains(t.HeldObject));

            DiverIsCarryingHeavyHoldable = new ReadOnlyReactiveProperty<bool>(isCarryingHeavyHoldable);
        }
    }
}