using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

namespace Player.Diver
{
    public class DiverFlashlight : MonoBehaviour
    {
        public float rotationSpeed = 20;
        private Quaternion _targetRot;
        public Light2D light;
        public DiverSmoothMovement diverSmoothMovement;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            light.enabled = true;
        }

        private void OnDisable()
        {
            light.enabled = false;
        }

        private void Update()
        {
            if (diverSmoothMovement.MoveDirection != Vector2.zero)
            {
                _targetRot = Quaternion.LookRotation(diverSmoothMovement.MoveDirection, Vector3.forward);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * rotationSpeed);
        }


        public void OnMove(InputAction.CallbackContext context)
        {
            context.ReadValue<Vector2>();
            var curRot = transform.rotation;
        }
    }
}