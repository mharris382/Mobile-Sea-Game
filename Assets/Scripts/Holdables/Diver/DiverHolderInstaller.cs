using JetBrains.Annotations;
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
            
            //NOTE: if it becomes necessary to inject the HoldableSelector this needs to be changed to BindToSelfAndInterfaces
            Container.Bind<Holder>().AsSingle();

            Container.DeclareSignal<DiverHeldItemChangedSignal>();
            Container.BindInterfacesTo<DiverHeldItemChangedSignal.Publisher>().AsSingle();
            
            
            //TODO: Move to a different installer
            Container.Bind<MonoBehaviour>().To<DiverSmoothMovement>().FromComponentsInHierarchy().WhenInjectedInto<StopMovementOnHoldHeavy>();
            Container.Bind<StopMovementOnHoldHeavy>().AsSingle();
            Container.BindSignal<DiverHeldItemChangedSignal>().ToMethod<StopMovementOnHoldHeavy>(t => t.OnHeldItemChanged).FromResolve();
        }

        

        [UsedImplicitly]
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