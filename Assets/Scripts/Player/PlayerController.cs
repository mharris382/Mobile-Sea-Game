using System;
using System.Linq;
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
        public InteractionTrigger trigger;
        public float diverGrabDistance = 1;
        [FormerlySerializedAs("isHoldingObject")] [SerializeField] bool _isHoldingObject;
        
        
        private StateMachine _fsm;
        private Holder objHolder;

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

            this.objHolder = GetComponent<Holder>();
            IState heavyMovement = new DiverHeavyMovement(diverMovement.GetComponent<Rigidbody2D>(), objHolder, heavyMoveConfig);
            IState normalMovement = diverMovement;
            
            _fsm.AddTransition(heavyMovement, normalMovement, () => _isHoldingObject == false);
            //TODO: update the transition so that it takes the weight of the object held into account before switching to heavy movement or if the object has a kinematic rigidbody2d
            _fsm.AddTransition(normalMovement, heavyMovement, () => _isHoldingObject == true);
            _fsm.SetState(normalMovement);
            moveAction.performed += (diverMovement as IListenForMoveInput).OnMove;
        }


        private void Update()
        {
            _fsm.Tick();
            var holdable = trigger.GetInRangeInteractables<IHoldable>().OrderBy(t => Vector2.Distance(t.rigidbody2D.position, diverMovement.rigidbody2D.position)).FirstOrDefault();
            if (holdable != null)
            {
                Debug.Log($"Available Holdable {holdable.name}!");
            }
        }

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


        public void OnInteract(InputAction.CallbackContext context)
        {
            if (_isHoldingObject)
            {
                _isHoldingObject = false;
                objHolder.ReleaseObject();
            }
            else
            {
                var holdable = trigger.GetInRangeInteractables<IHoldable>().FirstOrDefault(t => t.CanBePickedUpBy(objHolder));
                if (holdable != null && objHolder.TryHoldObject(holdable,
                    new Holder.SpringJointHolder(holdable.rigidbody2D, diverMovement.rigidbody2D, Vector2.Distance(diverMovement.rigidbody2D.position, holdable.rigidbody2D.position))))
                {
                    _isHoldingObject = true;
                }
            }
        }
    }


    public interface IListenForMoveInput
    {
        void OnMove(InputAction.CallbackContext context);
    }
}