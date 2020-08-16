using System;
using Holdables;
using UniRx;
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
        private IDisposable _disposable;
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
           //_disposable = MessageBroker.Default.Receive<HookHeldItemChangedSignal>().Subscribe(Callback);
            // try
            // {
            //     _signalBus.Subscribe<HookHeldItemChangedSignal>(Callback);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError($"Exception thrown subscribing from HookHeldItemChangedSignal\n{e.GetType().Name}: {e.Message}",this);
            // }
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
            // try
            // {
            //     _signalBus.Unsubscribe<HookHeldItemChangedSignal>( Callback);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError($"Exception thrown unSubscribing from HookHeldItemChangedSignal\n{e.GetType().Name}: {e.Message}",this);
            // }
        }

        // public void Callback(HookHeldItemChangedSignal obj)
        // {
        //     if (obj.HeldObject == _holdable)
        //     {
        //         Debug.Log($"HookAttachable {name} was picked up!");
        //         _sr.sprite = attachedSprite;
        //     }
        //     else if(revertOnDetached && _sr.sprite == attachedSprite)
        //     {
        //         Debug.Log($"HookAttachable {name} was released!");
        //         _sr.sprite = _detachedSprite;
        //     }
        // }
    }
}