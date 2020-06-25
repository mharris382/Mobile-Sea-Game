using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Core.State
{
    public delegate void StateChangeOccured(IState previousState, IState newState);
    public class StateMachine
    {
        private List<StateTransition> _stateTransitions = new List<StateTransition>();
        private List<StateTransition> _anyStateTransitions = new List<StateTransition>();
        
        private IState _currentState;
        private IEnumerator _stateChange;
        private bool _isChangingStates = false;

        public event StateChangeOccured OnStateChanged;
        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            var transition = new StateTransition(from, to, condition);
            _stateTransitions.Add(transition);
        }


        public void AddTransitions(IState to, Func<bool> condition, params IState[] states)
        {
            foreach (var state in states)
            {
                AddTransition(state, to, condition);
            }
        }
    
        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            var transition = new StateTransition(null, to, condition);
            _anyStateTransitions.Add(transition);
        }

        public void Tick()
        {
            if (_isChangingStates) return;
            var transition = CheckForTransitions();

            if (transition != null)
            {
                SetState(transition.To);
            }
        
            _currentState.Tick();
        }

        public void FixedTick()
        {
            _currentState?.FixedTick();
        }

        public void SetState(IState state)
        {
            if (_currentState == state) return;
        
            if(_isChangingStates) 
                CoroutineHandler.instance.StopCoroutine(_stateChange);

            
            CoroutineHandler.instance.StartCoroutine(_stateChange = ChangeStates(_currentState, state));
            
            // _currentState?.OnStateExit();
            //
            // _currentState = state;
            //
            // Debug.LogWarning($"Current State: {_currentState}");
            //
            // _currentState.OnStateEnter();
        }

        IEnumerator ChangeStates(IState from, IState to)
        {
            _isChangingStates = true;
            
            if(from != null)
                yield return from.OnStateExit();

            _currentState = to;
            yield return to.OnStateEnter();
            
            _isChangingStates = false;
            OnStateChanged?.Invoke(from, to);
        }

        private StateTransition CheckForTransitions()
        {
            foreach (var anyTransition in _anyStateTransitions)
            {
                if (anyTransition.Condition())
                    return anyTransition;
            }

            foreach (var transition in _stateTransitions)
            {
                if (transition.From == _currentState && transition.Condition())
                    return transition;
            }

            return null;
        }
    }
}