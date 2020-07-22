using UnityEngine;
using Zenject;

namespace Holdables.Diver
{
    public class DiverHolderInstaller : MonoInstaller<DiverHolderInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Rigidbody2D>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Holder>().AsSingle();

            Container.BindInterfacesTo<DiverHolderSignalPublisher>().AsSingle();
            
            Container.DeclareSignal<DiverHeldItemChangedSignal>();
            Container.BindSignal<DiverHeldItemChangedSignal>().ToMethod(t =>
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

        private class DiverHolderSignalPublisher : IInitializable
        {
            private SignalBus _signalBus;
            private Holder _holder;

            public DiverHolderSignalPublisher(SignalBus signalBus, Holder holder)
            {
                _signalBus = signalBus;
                _holder = holder;
            }

            public void Initialize()
            {
                _holder.OnPickedUp += holdable => _signalBus.Fire(new DiverHeldItemChangedSignal(holdable));
                _holder.OnReleased += holdable => _signalBus.Fire(new DiverHeldItemChangedSignal(null));
            }
        }
    }
}