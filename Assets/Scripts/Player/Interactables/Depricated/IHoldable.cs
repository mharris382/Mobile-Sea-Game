namespace Player
{
    [System.Obsolete("Now Use Holdables.IHoldable")]
    public interface IHoldable : IInteractable
    {
        bool CanBePickedUpBy(Holder holder);

        
        void OnPickedUp(Holder holder);
        void OnReleased();
    }

    [System.Obsolete("Not using anymore")]
    public interface IHeavyHoldable
    {
        
    }
}