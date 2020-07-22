using Player.Diver;
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



            //TODO: Move to a different installer
            Container.Bind<MonoBehaviour>().To<DiverSmoothMovement>().FromComponentsInHierarchy().WhenInjectedInto<StopMovementOnHoldHeavy>();
            Container.Bind<StopMovementOnHoldHeavy>().AsSingle();
            Container.BindSignal<DiverHeldItemChangedSignal>().ToMethod<StopMovementOnHoldHeavy>(t => t.OnHeldItemChanged).FromResolve();
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


        public class StopMovementOnHoldHeavy
        {
            private readonly MonoBehaviour _movement;

            private bool _holdingHeavyObject;

            public StopMovementOnHoldHeavy(MonoBehaviour movement)
            {
                _movement = (MonoBehaviour) movement;
            }

            public void OnHeldItemChanged(DiverHeldItemChangedSignal signal)
            {
                if (signal.HeldObject == null)
                {
                    if (!_holdingHeavyObject)
                        return;

                    SetHoldingHeavy(false);
                }
                else if (IsPickupHeavy(signal))
                {
                    SetHoldingHeavy(true);
                }
            }

            private bool IsPickupHeavy(DiverHeldItemChangedSignal signal)
            {
                if (signal.HeldObject.rigidbody2D == null) return false;
                return signal.HeldObject.rigidbody2D.CompareTag("Heavy");
                //return signal.HeldObject.rigidbody2D != null && signal.HeldObject.rigidbody2D.mass >= _weightLimit;
            }

            private void SetHoldingHeavy(bool isHoldingHeavy)
            {
                _holdingHeavyObject = isHoldingHeavy;
                _movement.enabled = !isHoldingHeavy;
            }
        }
    }
}