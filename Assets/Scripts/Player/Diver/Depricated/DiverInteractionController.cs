using Core;
using UnityEngine;

namespace Player.Diver
{
    [RequireComponent(typeof(Holder), typeof(Rigidbody2D))]
    public class DiverInteractionController : MonoBehaviour
    {
        [SerializeField] private InteractionTrigger _interactionTrigger;
        [SerializeField] private Transform _jointAttachPoint;

        private Holder _holder;
        private DiverPickupHandler _pickupHandler;


        private void Awake()
        {
            if (_interactionTrigger == null) _interactionTrigger = GetComponentInChildren<InteractionTrigger>();
            if (_jointAttachPoint == null)
            {
                _jointAttachPoint = new GameObject("AttachPoint").transform;
                _jointAttachPoint.parent = transform;
            }

            _holder = GetComponent<Holder>();

            var jointFactory = new DiverJointHolderFactory(GetComponent<Rigidbody2D>(), _jointAttachPoint);
            _pickupHandler = new DiverPickupHandler(_holder, jointFactory);


            GameInput.DiverGameplayActions.Interact.performed += context =>
            {
                if (_interactionTrigger.HasTypeInRange<IHoldable>())
                {
                    _pickupHandler.OnInteract(_interactionTrigger.GetInRangeInteractables<IHoldable>());
                }
            };
        }
    }
}