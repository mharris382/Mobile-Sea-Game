using JetBrains.Annotations;
using UnityEngine;

namespace Holdables.Diver
{
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