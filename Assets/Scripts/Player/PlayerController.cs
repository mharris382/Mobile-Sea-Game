using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Core;
using Core.State;
using Player.Diver;
using Player.Sub;
using PolyNav;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using static Player.Holder;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public DiverSmoothMovement diverMovement;
        public DiverHeavyMovement.Config heavyMoveConfig;
        public PlayerInput input;
        public GameObject triggerGameObject;
        public Transform fixedAttachPoint;


        private IDetectInteractions _trigger;
        

        public DiverPickupHandler DiverPickupHandler;
        private StateMachine _diverFsm;
        private Holder _objHolder;

        public bool isHoldingObject
        {
            get => _objHolder.IsHoldingObject;
        }

        private void Awake()
        {
            _trigger = triggerGameObject.GetComponent<IDetectInteractions>();
            this._objHolder = GetComponent<Holder>() ?? diverMovement.GetComponent<Holder>();
            _diverFsm = new StateMachine();

            IState heavyMovement = null;//new DiverHeavyMovement(diverMovement.GetComponent<Rigidbody2D>(), _objHolder, heavyMoveConfig);
            IState normalMovement = diverMovement;
            
            
            _diverFsm.SetState(normalMovement);
           // moveAction.performed += (diverMovement as IListenForMoveInput).OnMove;
        }


        private void Update()
        {
            _diverFsm.Tick();
            
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
           
        }

        //TODO: Refactor the pickup code into separate class
        public void OnInteract(InputAction.CallbackContext context)
        {
            InputActionPhase phase = context.phase;

            if (context.performed == false) return;
            
            if (isHoldingObject)
            {
                DiverPickupHandler.DropHeldObjects();
            }
            else
            {
                DiverPickupHandler.OnInteract(_trigger.GetInRangeInteractables<IHoldable>().ToArray());
                // IHoldable holdable = trigger.GetInRangeInteractables<IHoldable>().FirstOrDefault(t => t.CanBePickedUpBy(objHolder));
                // if (holdable != null  && objHolder.TryHoldObject(holdable,  GetHoldJoint(holdable.rigidbody2D)))
                // {
                //     //_isHoldingObject = true;
                //     return;
                // }
            }
        }
        

    }
}