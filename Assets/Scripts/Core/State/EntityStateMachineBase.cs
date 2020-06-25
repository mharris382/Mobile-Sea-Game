using System;
using UnityEngine;

namespace Core.State
{
    public abstract class EntityStateMachineBase : MonoBehaviour
    {
        private StateMachine _stateMachine;
    
    
        private void Start()
        { 
            _stateMachine = new StateMachine();
            InitStateMachine(ref _stateMachine);
        }
    
        protected abstract void InitStateMachine(ref StateMachine fsm);
    
    }
}