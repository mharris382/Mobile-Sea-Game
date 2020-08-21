using System;
using Holdables;
using Hook.Signals;
using UniRx;
using UnityEngine;

namespace Hook
{
    // ReSharper disable once InconsistentNaming
    public class UI_NotifyHookAttachmentAvailable : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI _notificationText;


        private int _frameCount = 0;
        
        private void Start()
        {
            MessageBroker.Default.Receive<HookHasAttachmentAvailableSignal>()
                .Where(t => t.holdable != null)
                .Subscribe(hookAttachment =>
                {
                    _frameCount = 0;
                    _notificationText.enabled = true;
                    _notificationText.text = $"Press Q to attach {hookAttachment.holdable.rigidbody2D.name} to Hook";
                } );
        }



         private void LateUpdate()
         {
             _frameCount++;
             if (_frameCount > 1)
             {
                 _notificationText.enabled = false;
             }
         }
    }
}