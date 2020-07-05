using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

namespace Player.Diver
{
    public class DiverFlashlight : MonoBehaviour, IFlashlight
    {
        public float rotationSpeed = 20;
        private Quaternion _targetRot;
        public GameObject light;
        public IDiverMovement diverSmoothMovement;
        public bool enableAutoToggleOnDepth = true;


        private void Start()
        {
            if (diverSmoothMovement == null)
                diverSmoothMovement = GetComponentInParent<IDiverMovement>();

            DepthTracker depthTracker;
            if (!diverSmoothMovement.rigidbody2D.TryGetComponent(out depthTracker))
            {
                depthTracker = gameObject.AddComponent<DepthTracker>();
            }
            GameManager.Instance.StartCoroutine(ToggleFlashlightOnDepth(depthTracker));
        }


        IEnumerator ToggleFlashlightOnDepth(DepthTracker depthTracker)
        {
            int depth = 0;

            void OnDepthTrackerOnOnDepthChanged(Depth depth1)
            {
                depth = depth1.feetBelowSeaLvl;
            }

            depthTracker.OnDepthChanged += OnDepthTrackerOnOnDepthChanged;
            
            while (true)
            {
                bool on = IsOn;
                if (enableAutoToggleOnDepth)
                {
                    on = depth > DiverConfig.FlashlightDepth;
                }
                IsOn = on;
                yield return new WaitForSeconds(1f);
            }
            
        }


        public bool IsOn
        {
            get => enabled;
            set => enabled = value;
        }

        private void OnEnable()
        {
            light.SetActive(true);
        }

        private void OnDisable()
        {
            light.SetActive(false);
        }

        private void Update()
        {
            if (diverSmoothMovement.MoveDirection != Vector2.zero)
            {
                _targetRot = Quaternion.LookRotation(diverSmoothMovement.MoveDirection, Vector3.forward);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * rotationSpeed);
        }


        [System.Obsolete]
        public void OnMove(InputAction.CallbackContext context)
        {
            context.ReadValue<Vector2>();
            var curRot = transform.rotation;
        }
    }

    public interface IFlashlight
    {
        bool IsOn { get; set; }
    }
}