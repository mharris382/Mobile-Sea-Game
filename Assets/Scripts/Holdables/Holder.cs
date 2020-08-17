using System;
using UniRx;
using UnityEngine;

namespace Holdables
{
    
    public class Holder : IHolder
    {
        private readonly Rigidbody2D _rigidbody2D;
        private IHoldable _heldObject;

        public IHoldable HeldObject
        {
            get => _heldObject;
            set => _heldObject = value;
        }
        
        
        
        public event Action<IHoldable> OnPickedUp;
        public event Action<IHoldable> OnReleased;
        
        
        public Holder(Rigidbody2D rigidbody2D)
        {
            _rigidbody2D = rigidbody2D;
        }

        public void PickupObject(IHoldable objectToHold)
        {
            if (_heldObject != null)
            {
                Debug.LogError($"{_rigidbody2D.name} is already holding {_heldObject.name}");
                return;
            }
            
            _heldObject = objectToHold;
            _heldObject.OnPickedUp(_rigidbody2D);
            OnPickedUp?.Invoke(_heldObject);
            
        }

        public void ReleaseHeldObject()
        {
            if (HeldObject != null)
            {
                var released = _heldObject;
                _heldObject = null;
                released.OnReleased();
                OnReleased?.Invoke(released);    
            }
        }

       
    }




   
    
}