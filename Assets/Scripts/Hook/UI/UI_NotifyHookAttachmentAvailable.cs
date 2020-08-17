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


        private int frameCount = 0;
        
        private void Start()
        {
            MessageBroker.Default.Receive<HookHasAttachmentAvailableSignal>().Subscribe(hookAttachment =>
            {
                if (hookAttachment.holdable == null)
                {
                    _notificationText.enabled = false;
                    return;
                }
                
                frameCount = 0;
                _notificationText.enabled = true;
                _notificationText.text = $"Press Q to attach {hookAttachment.holdable.rigidbody2D.name} to Hook";
                
                Debug.Log("Display");
            } );
        }



        private void LateUpdate()
        {
            frameCount++;
            if (frameCount > 1 && _notificationText.enabled)
            {
                Debug.Log("Clear");
                _notificationText.enabled = false;
            }
            else if(_notificationText.enabled)
            {
                Debug.Log("Wait");
            }
        }
    }
}