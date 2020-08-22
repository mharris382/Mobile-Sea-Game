using Diver;
using Signals;
using Sirenix.OdinInspector;
using UniRx;
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

        // [InjectOptional(Id = "StartLength")]
        private float _currentDistance;
    
        private DiverInput _input;
        // [SerializeField,Required]
        // private UI_HookIndicator _uiHookIndicator;



        [Inject]
        void Inject(DiverInput input, RopeTest rope)
        {
            this._input = input;
            this._rope = rope;
        }

        private void Awake()
        {
            _currentDistance = _rope.startLength;
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
            MessageBroker.Default.Publish(new RopeLengthChangedSignal(){ropeLength = _currentDistance});
            // _uiHookIndicator.RopeLength = _currentDistance;
        }
    }
}