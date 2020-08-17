using System;
using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace Holdables.Diver
{
    [InjectedBy(typeof(DiverHolderInstaller))]
    [UsedImplicitly()]
    public class DiverPickupAvailableSignalProvider : ITickable
    {
        private HoldableProvider _provider;
        private Holder _holder;


        [Inject]
        void Inject(Holder holder, HoldableProvider provider)
        {
            this._holder = holder;
            this._provider = provider;
        }

        public void Tick()
        {
            var available = _holder.HeldObject != null ? null : _provider.GetFirstChoiceForPickup();
            if (available!=null)
            {
                MessageBroker.Default.Publish(new Signals.DiverPickupAvailableSignal(available));
            }
        }
    }
    
    
}


public class InjectedByAttribute : Attribute
{
    private Type _installerType;

    public InjectedByAttribute(Type installerType)
    {
        this._installerType = installerType;
    }
}