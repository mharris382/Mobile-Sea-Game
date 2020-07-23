using System;
using Holdables;
using UnityEngine;
using Utilities;
using Zenject;

namespace Hook
{
    public class HookAttachable : MonoBehaviour
    {
        public bool revertOnDetached;
        public Sprite attachedSprite;
        private Sprite _detachedSprite;
        private SignalBus _signalBus;
        private SpriteRenderer _sr;

        private IHoldable _holdable;

        [Inject]
        void Install(SignalBus signalBus)
        {
            this._signalBus = signalBus;
        }


        private void Awake()
        {
            _holdable = GetComponent<IHoldable>();
            if(_holdable == null)Debug.LogError("Missing IHoldable on HookAttachable", this);
            _sr = GetComponentInChildren<SpriteRenderer>();
            _detachedSprite = _sr.sprite;
        }


        private void Start()
        {
            //_signalBus.Subscribe<HookHeldItemChangedSignal>(Callback);
        }

        private void OnDestroy()
        {
            //_signalBus.Unsubscribe<HookHeldItemChangedSignal>( Callback);
        }

        public void Callback(HookHeldItemChangedSignal obj)
        {
            if (obj.HeldObject == _holdable)
            {
                Debug.Log($"HookAttachable {name} was picked up!");
                _sr.sprite = attachedSprite;
            }
            else if(revertOnDetached && _sr.sprite == attachedSprite)
            {
                Debug.Log($"HookAttachable {name} was released!");
                _sr.sprite = _detachedSprite;
            }
        }
    }
}