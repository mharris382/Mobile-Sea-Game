using System;
using Core;
using Core.State;
using UnityEngine;
using DiverActions = UnderTheSeaInput.DiverGameplayActions;
namespace Player.Diver
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Diver : MonoBehaviour
    {
        private StateMachine _fsm;
        private DiverActions _diverActions;
        private Holder _holder;
        private Rigidbody2D _rigidbody2D;
        private ClampToLevel _positionConstraint;
        
        private void Awake()
        {
            _fsm = new StateMachine();
            _diverActions = GameInput.DiverGameplayActions;
            _positionConstraint = new ClampToLevel();
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
            if (!TryGetComponent(out _holder)) _holder = gameObject.AddComponent<Holder>();
            
            
            var normalDiverMovement = new DiverMovement(_rigidbody2D, _diverActions);
            var heavyDiverMovement = new DiverHeavyMovement(_rigidbody2D, _diverActions);
            
            var carryObjCondition = new IsDiverCarryingHeavyObjectCondition(_holder);
            _fsm.AddTransition(normalDiverMovement, heavyDiverMovement, () => carryObjCondition.EvaluateCondition());
            _fsm.AddTransition(heavyDiverMovement, normalDiverMovement, () => !carryObjCondition.EvaluateCondition());
            
            _fsm.SetState(normalDiverMovement);
        }

        private void Update()
        {
            _fsm.Tick();
        }

        private void FixedUpdate()
        {
            _fsm.FixedTick();
            _rigidbody2D.position = _positionConstraint.ClampPositionToLevel(_rigidbody2D.position);
        }


        private void OnEnable() => _diverActions.Disable();

        private void OnDisable() => _diverActions.Disable();
    }
}