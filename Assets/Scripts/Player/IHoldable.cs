namespace Player
{
    public interface IHoldable : IInteractable
    {
        bool CanBePickedUpBy(Holder holder);


        void Pickup(Holder holder);
        void Release();
    }

}