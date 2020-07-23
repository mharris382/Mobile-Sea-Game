using System;
using Holdables;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Hook
{
    public class HookHeldItemChangedSignal
    {
        public IHoldable HeldObject { get; private set; }

        public HookHeldItemChangedSignal(IHoldable heldObj)
        {
            HeldObject = heldObj;
        }
        
        
        
        [UsedImplicitly]
        private class Publisher : IInitializable
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
                _holder.OnPickedUp += holdable => _signalBus.Fire(new HookHeldItemChangedSignal(holdable));
                _holder.OnReleased += holdable => _signalBus.Fire(new HookHeldItemChangedSignal(null));
            }
        }
        
        [UsedImplicitly]
        public class Installer : Installer<HookHeldItemChangedSignal.Installer>
        {
            public override void InstallBindings()
            {
                Container.BindInterfacesTo<HookHeldItemChangedSignal.Publisher>().AsSingle();
                Container.DeclareSignal<HookHeldItemChangedSignal>();
            
                Container.BindSignal<HookHeldItemChangedSignal>().ToMethod(t =>
                {
                    if (t.HeldObject == null)
                    {
                        Debug.Log($"Diver is no longer holding anything".InBold());
                    }
                    else
                    {
                        Debug.Log($"Diver is now holding {t.HeldObject.name}".InBold());
                    }
                });
            }
        }
    }
}