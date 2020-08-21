using Diver;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Hook
{
    [TypeInfoBox("Listens to input and increases/decreases rope length to lower/raise the hook")]
    public class RopeLengthController : MonoBehaviour
    {
        private RopeTest _rope;


        public float speed = 5;
        public float minRopeDistance = 1;
        public float maxRopeDistance = 50;

        private float _currentDistance;
    
        private DiverInput _input;
        [SerializeField,Required]
        private UI_HookIndicator _uiHookIndicator;



        [Inject]
        void Inject(DiverInput input)
        {
            this._input = input;
        }

        private void Awake()
        {
            _rope = GetComponent<RopeTest>();
           // _uiHookIndicator = GetComponent<UI_HookIndicator>();
           // Debug.Assert(_uiHookIndicator != null, "Missing UI Hook Indicator", this);
        }

        private void Update()
        {
            var moveInput = _input.HookMoveInput;
        
            if(Mathf.Abs(moveInput) < 0.1f)
                return;
        
            // if((moveInput < 0 && _currentDistance >= maxRopeDistance) || 
            //    (moveInput > 0) && _currentDistance <= minRopeDistance)
            //     return;

            var moveAmount = Time.deltaTime * speed * moveInput;
            _currentDistance += -moveAmount;
            _rope.Distance = _currentDistance;
            _uiHookIndicator.RopeLength = _currentDistance;
        }
    }
}