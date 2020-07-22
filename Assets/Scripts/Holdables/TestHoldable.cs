using System;
using UnityEngine;

namespace Holdables
{
    public class TestHoldable : MonoBehaviour , IHoldable
    {
        
        private Collider2D _coll;

        public new Rigidbody2D rigidbody2D { get; private set; }

        public bool IsHeld { get; private set; }

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
        }

        public void OnPickedUp(Rigidbody2D holderBody)
        {
            IsHeld = true;
            _coll.enabled = false;
        }

        public void OnReleased()
        {
            IsHeld = false;
            _coll.enabled = true;
        }
    }
}