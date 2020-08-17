using UnityEngine;

namespace Holdables
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class HoldableObject : MonoBehaviour , IHoldable
    {
        private bool isHeld = false;
        private Rigidbody2D _rb;
        private Collider2D _coll;


        private int _holderCnt=0;
        public bool IsHeld => isHeld;

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
      

        public void OnPickedUp(Rigidbody2D holderBody)
        {
            isHeld = true;
            _holderCnt ++;
        }

        public void OnReleased()
        {
            isHeld = false;
            if (_holderCnt > 0)
            {
                _holderCnt--;
            }
        }
    }
}