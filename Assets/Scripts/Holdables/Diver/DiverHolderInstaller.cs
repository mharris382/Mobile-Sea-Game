using System;
using Player;
using Player.Diver;
using UniRx;
using UnityEngine;
using Zenject;

namespace Holdables.Diver
{
    public class DiverHolderInstaller : MonoInstaller<DiverHolderInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Rigidbody2D>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Transform>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IInteractionTrigger>().To<InteractionTrigger>().FromComponentInHierarchy().AsSingle();
            Container.Bind<HoldableProvider>().AsSingle();
            
            //NOTE: if it becomes necessary to inject the HoldableSelector this needs to be changed to BindToSelfAndInterfaces
            Container.Bind(typeof(IHold), typeof(Holder)).To<Holder>().AsSingle();
            Container.DeclareSignal<DiverHeldItemChangedSignal>();
            Container.BindInterfacesTo<DiverHeldItemChangedSignal.Publisher>().AsSingle();
            Container.BindInterfacesTo<DiverPickupAvailableSignalProvider>().AsSingle().NonLazy();

            //TODO: Move to a different installer
            Container.Bind<MonoBehaviour>().To<DiverSmoothMovement>().FromComponentsInHierarchy().WhenInjectedInto<StopMovementOnHoldHeavy>();
            Container.Bind<StopMovementOnHoldHeavy>().AsSingle();
            Container.BindSignal<DiverHeldItemChangedSignal>().ToMethod<StopMovementOnHoldHeavy>(t => t.OnHeldItemChanged).FromResolve();

            //Container.BindInterfacesTo<HeavyHoldableListener>().AsSingle();
        }

        
        

    }
}