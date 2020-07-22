using System;
using Player;
using UnityEngine;

namespace Holdables
{
    public interface IPickup
    {
        void PickupObject(IHoldable objectToHold);
    }

    public interface IRelease
    {
        void ReleaseHeldObject();
    }
    
    
    public interface IHold : IHoldEvents
    {
        IHoldable HeldObject { get; }
    }

    

    public interface IHoldEvents
    {
        event Action<IHoldable> OnPickedUp;
        event Action<IHoldable> OnReleased;
    }

    public interface IHolder : IPickup, IRelease, IHold
    {
        
    }
    
    public interface IHoldable : IInteractable
    {
        bool IsHeld { get; }
        void OnPickedUp(Rigidbody2D holderBody);
        void OnReleased();
    }



    public static class HoldableExtensions
    {
        public static bool IsHeld(this IHoldable holdable)
        {
            return holdable.IsHeld;
        }
    }
    
}