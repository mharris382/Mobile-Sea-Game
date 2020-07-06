using System;
using UnityEngine;
namespace Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class HoldableObject : MonoBehaviour , IHoldable
    {
        private bool isHeld = false;
        private Rigidbody2D _rb;
        private Collider2D _coll;


        public new Rigidbody2D rigidbody2D => _rb;

        private void Awake()
        {
            this._rb = GetComponent<Rigidbody2D>();
            this._coll = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            _coll.enabled = true;
        }
        
        private void OnDisable()
        {
            _coll.enabled = false;
        }

        public bool CanBePickedUpBy(Holder holder)
        {
            return isHeld == false;
        }

        public virtual void OnPickedUp(Holder holder)
        {
            isHeld = true;
        }

        public virtual void OnReleased()
        {
            isHeld = false;
        }
    }
}