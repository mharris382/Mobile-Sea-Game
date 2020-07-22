using UnityEngine;

namespace Player
{
    [System.Obsolete("Now using Holdables.HoldableObject which is sealed")]
    public class TreasureChestPickup : HoldableObject, IHeavyHoldable
    {
        public Sprite pickedUpSprite;
        private SpriteRenderer _sr;
        private Collider2D _coll;
        private void Start()
        {
            _coll = GetComponentInChildren<Collider2D>();
            _sr = GetComponentInChildren<SpriteRenderer>();
            Debug.Assert(_coll.density > 2, $"Are you sure you want this chest to float (Density = {_coll.density})?");
        }

        public override void OnPickedUp(Holder holder)
        {
            base.OnPickedUp(holder);
            
            if(pickedUpSprite != null)
                _sr.sprite = pickedUpSprite;
        }
    }
}