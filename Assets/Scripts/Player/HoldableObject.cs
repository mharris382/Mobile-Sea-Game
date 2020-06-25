using UnityEngine;
namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class HoldableObject : MonoBehaviour , IHoldable
    {
        private Rigidbody2D _rb;
        public new Rigidbody2D rigidbody2D => _rb;

        public bool CanBePickedUpBy(Holder holder)
        {
            return isHeld == false;
        }

        private bool isHeld = false;
        public void Pickup(Holder holder)
        {
            isHeld = true;
        }

        public void Release()
        {
            isHeld = false;
        }

        private void Awake()
        {
            this._rb = GetComponent<Rigidbody2D>();
        }
    }

}