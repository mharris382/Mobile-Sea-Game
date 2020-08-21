using System;
using Signals;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Holdables.Diver
{
    public class UI_PickupAvailableText : MonoBehaviour
    {
        [Required]
        public TMPro.TextMeshProUGUI notificationText;
        
        private int _cnt = 0;
        
        private void Start()
        {
           
            MessageBroker.Default.Receive<DiverPickupAvailableSignal>()
                .Where(t => t.availablePickup != null)
                .Subscribe(t =>
                {
                    Debug.Log($"Diver has {t.availablePickup.name} available.");
                    _cnt = 0;
                    notificationText.enabled = true;
                    notificationText.text = $"Press E to pickup {t.availablePickup.name}";
                });
            
            DisableNotificationText();
        }

        private void LateUpdate()
        {
            _cnt++;
            if (_cnt <= 1) return;
            DisableNotificationText();
        }

        private void DisableNotificationText()
        {
            notificationText.text = "";
            notificationText.enabled = false;
        }
    }
}
