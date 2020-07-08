namespace Player.Diver
{
    public class IsDiverCarryingHeavyObjectCondition
    {
        private Holder _holder;
        private readonly float _weightThreshold;


        public IsDiverCarryingHeavyObjectCondition(Holder holder, float weightThreshold = 1)
        {
            _holder = holder;
            _weightThreshold = weightThreshold;
        }

        public bool EvaluateCondition()
        {
            if (_holder.IsHoldingObject == false)
            {
                return false;
            }

            if (_holder.HeldObject.rigidbody2D.isKinematic)
                return false;

            return _holder.HeldObject.rigidbody2D.mass > _weightThreshold;
        }
    }
}