using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Holdables.Diver
{
    public class UI_PickupAvailableText : MonoBehaviour
    {
        [Required]
        public TMPro.TextMeshProUGUI notificationText;
        
        
        private HoldableProvider _provider;
        private Holder _holder;


        [Inject]
        void Inject(Holder holder, HoldableProvider provider)
        {
            this._holder = holder;
            this._provider = provider;
        }


        private void Start()
        {
            Debug.Assert(_holder != null, "Null Holder injected", this);
            Debug.Assert(_provider != null, "Null Provider Injected", this);
            
            notificationText.enabled = true;
            notificationText.text = "";
            transform.parent = null;
        }

        private void Update()
        {
            var available = _holder.HeldObject != null ? null : _provider.GetFirstChoiceForPickup();
            if (available == null)
            {
                notificationText.text = "";
            }
            else
            {
                
                notificationText.text = $"Press E to pickup {available.name}";
            }
        }

    }
}
