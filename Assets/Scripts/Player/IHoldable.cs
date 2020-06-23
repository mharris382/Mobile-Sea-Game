namespace Player
{
    public interface IHoldable
    {
        bool CanBePickedUpBy(Holder holder);
    }

}