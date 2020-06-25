using System;
using Cinemachine;
using Core;
using Core.State;
using Player.Diver;
using Player.Sub;
using PolyNav;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public DiverSmoothMovement diverMovement;
        public DiverHeavyMovement.Config heavyMoveConfig;
        public PlayerInput input;
        [FormerlySerializedAs("isHoldingObject")] [SerializeField] bool _isHoldingObject;
        
        
        private StateMachine _fsm;

        public bool isHoldingObject
        {
            get => _isHoldingObject;
            set => _isHoldingObject = value;
        }

        private void Awake()
        {
            _fsm = new StateMachine();
            
            var moveAction = input.actions["move"];
            _fsm.OnStateChanged += (state, newState) =>
            {
                if (state is IListenForMoveInput)
                {
                    var prevListener = state as IListenForMoveInput;
                    moveAction.performed -= prevListener.OnMove;
                }

                if (newState is IListenForMoveInput)
                {
                    var nextListener = newState as IListenForMoveInput;
                    moveAction.performed += nextListener.OnMove;
                }
                else
                {
                    Debug.LogWarning("No State is listening for move input");
                }
            };

            Holder objHolder = GetComponent<Holder>();
            IState heavyMovement = new DiverHeavyMovement(diverMovement.GetComponent<Rigidbody2D>(), objHolder, heavyMoveConfig);
            IState normalMovement = diverMovement;

            _fsm.AddTransition(heavyMovement, normalMovement, () => _isHoldingObject == false);
            _fsm.AddTransition(normalMovement, heavyMovement, () => _isHoldingObject == true);
            _fsm.SetState(normalMovement);
            moveAction.performed += (diverMovement as IListenForMoveInput).OnMove;
        }


        private void Update() => _fsm.Tick();

        private void FixedUpdate() => _fsm.FixedTick();

        private void LateUpdate() => ClampPositionToLevel();


        private void ClampPositionToLevel( )
        {
            Vector3 position = diverMovement.rigidbody2D.position;
            var rect = GameManager.Instance.CurrentLevel.GetLevelRect();

            position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
            position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);
            diverMovement.rigidbody2D.position = position;
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            var moveDirection = context.ReadValue<Vector2>().normalized;
           
        }
    }


    public interface IListenForMoveInput
    {
        void OnMove(InputAction.CallbackContext context);
    }
}