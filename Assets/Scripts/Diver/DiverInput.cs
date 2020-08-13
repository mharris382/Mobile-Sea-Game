using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Diver
{
    public class DiverInput: MonoBehaviour
    {
        public bool useInputClassInstance = false;

        [HideIf("useInputClassInstance"),Required] public PlayerInput playerInput;

        private InputAction _moveAction;
        private InputAction _moveHookAction;

        public float HookMoveInput { private set; get; }
        public Vector2 DiverMoveInput { private set; get; }
        
        private void Start()
        {
            InputAction interact=null, useHook=null, toggleSprint =null;
            if (useInputClassInstance)
            {
                var m_controls = new UnderTheSeaInput();
                throw new NotImplementedException();
            }
            else
            {
                _moveAction = GetActionSafe("Move");
                _moveHookAction = GetActionSafe("MoveHook");
                interact = GetActionSafe("Interact");
                useHook = GetActionSafe("Hook");
                toggleSprint = GetActionSafe("ToggleFastMove");
            }

            interact.started += context => { };
        }

        private InputAction GetActionSafe(string moveHookName)
        {
            var moveHookAction = playerInput.actions[moveHookName];
            Debug.Assert(_moveHookAction != null, $"No Action named {moveHookName} found!");
            return moveHookAction;
        }


        private void Update()
        {
            this.DiverMoveInput = _moveAction.ReadValue<Vector2>();
            this.HookMoveInput = _moveHookAction.ReadValue<float>();
            
        }
    }
}