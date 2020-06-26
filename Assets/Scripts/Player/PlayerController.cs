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
using static Player.Holder;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public DiverSmoothMovement diverMovement;
        public DiverHeavyMovement.Config heavyMoveConfig;
        public PlayerInput input;
        public InteractionTrigger trigger;
        public Transform fixedAttachPoint;
        public float diverGrabDistance = 1;
        [FormerlySerializedAs("isHoldingObject")] [SerializeField] bool _isHoldingObject;
        
        
        private StateMachine _diverFsm;
        private Holder objHolder;

        public bool isHoldingObject
        {
            get => _isHoldingObject;
            set => _isHoldingObject = value;
        }

        private void Awake()
        {
            this.objHolder = GetComponent<Holder>();
            _diverFsm = new StateMachine();
            
            var moveAction = input.actions["move"];
            _diverFsm.OnStateChanged += (state, newState) =>
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

            
            IState heavyMovement = new DiverHeavyMovement(diverMovement.GetComponent<Rigidbody2D>(), objHolder, heavyMoveConfig);
            IState normalMovement = diverMovement;
            
            _diverFsm.AddTransition(heavyMovement, normalMovement, () => _isHoldingObject == false);
            _diverFsm.AddTransition(normalMovement, heavyMovement, () => _isHoldingObject && (objHolder.HeldObject != null) && (objHolder.HeldObject.rigidbody2D.isKinematic == false));
            
            
            _diverFsm.SetState(normalMovement);
            moveAction.performed += (diverMovement as IListenForMoveInput).OnMove;
        }


        private void Update()
        {
            _diverFsm.Tick();
            var holdable = trigger.GetInRangeInteractables<IHoldable>().OrderBy(t => Vector2.Distance(t.rigidbody2D.position, diverMovement.rigidbody2D.position)).FirstOrDefault();
            if (holdable != null)
            {
                Debug.Log($"Available Holdable {holdable.name}!");
            }
        }

        private void FixedUpdate() => _diverFsm.FixedTick();

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

        //TODO: Refactor the pickup code into separate class
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (_isHoldingObject)
            {
                _isHoldingObject = false;
                objHolder.ReleaseObject();
            }
            else
            {
                IHoldable holdable = trigger.GetInRangeInteractables<IHoldable>().FirstOrDefault(t => t.CanBePickedUpBy(objHolder));
                
                if (holdable != null  && objHolder.TryHoldObject(holdable,  GetHoldJoint(holdable.rigidbody2D)))
                {
                    _isHoldingObject = true;
                    return;
                }
            }
        }
        private JointHolderBase GetHoldJoint(Rigidbody2D holdable)
        {
            float distance = Vector2.Distance(diverMovement.rigidbody2D.position, holdable.position);
            
            JointHolderBase holdJoint = !holdable.isKinematic ?  (JointHolderBase)
               new SpringJointHolder(holdable, diverMovement.rigidbody2D, distance) :
               new FixedJointHolder(holdable, diverMovement.rigidbody2D, fixedAttachPoint, distance);

            return holdJoint;
        }
    }


    public interface IListenForMoveInput
    {
        void OnMove(InputAction.CallbackContext context);
    }
}