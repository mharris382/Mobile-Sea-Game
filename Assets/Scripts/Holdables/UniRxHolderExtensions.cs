using UniRx;

namespace Holdables
{
    public static class UniRxHolderExtensions
    {
        public static IObservable<IHoldable> OnPickupAsObservable(this Holder holder)
        {
            return Observable.FromEvent<IHoldable>(
                t => holder.OnPickedUp += t,
                t => holder.OnPickedUp -= t);
        }
        public static IObservable<IHoldable> OnReleasedAsObservable(this Holder holder)
        {
            return Observable.FromEvent<IHoldable>(
                t => holder.OnReleased += t,
                t => holder.OnReleased -= t);
        }
    }
}