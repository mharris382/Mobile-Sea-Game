using System;
using Core;
using Diver;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Hook
{
    
    public class BoatMover : MonoBehaviour
    {
        public Rigidbody2D boatBody;
        public float moveSpeed = 25;
        private DiverInput _diverInput;
        [SerializeField,Required]
        private UI_HookIndicator _uiHookIndicator;

        [Inject]
        void Inject(DiverInput input)
        {
            this._diverInput = input;
        }

        private void Awake()
        {
            //_uiHookIndicator = GetComponent<UI_HookIndicator>();
            //Debug.Assert(_uiHookIndicator != null, "Missing UI Hook Indicator", this);
        }

        private void Update()
        {
            if(Mathf.Abs(_diverInput.BoatMoveInput) < 0.1f)
                return;

            Vector2 moveAmount = Vector2.right * (_diverInput.BoatMoveInput * moveSpeed * Time.deltaTime);
            boatBody.position += moveAmount;
            
            ClampPosition();
            _uiHookIndicator.BoatPosition = boatBody.position;
        }

        private void ClampPosition()
        {
            void ClampPositionToLevel(ref Vector3 position)
            {
                var rect = GameManager.Instance.CurrentLevel.GetLevelRect();

                position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
                position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);
            }

            Vector3 currPosition = boatBody.position;
            ClampPositionToLevel(ref currPosition);
            boatBody.position = currPosition;
        }
       
    }
}