namespace Holdables.Diver
{
    public class DiverHeldItemChangedSignal
    {
        public DiverHeldItemChangedSignal(IHoldable heldObject)
        {
            HeldObject = heldObject;
        }

        public IHoldable HeldObject { get; private set; }
    }
}