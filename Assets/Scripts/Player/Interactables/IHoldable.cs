namespace Player
{
    public interface IHoldable : IInteractable
    {
        bool CanBePickedUpBy(Holder holder);

        
        void OnPickedUp(Holder holder);
        void OnReleased();
    }

    public interface IHeavyHoldable
    {
        
    }
}