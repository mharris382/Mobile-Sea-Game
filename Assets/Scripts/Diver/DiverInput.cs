
using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Diver
{
    public class DiverInput: MonoBehaviour
    {
        public bool useInputClassInstance = false;

        [HideIf("useInputClassInstance"),Required] 
        public PlayerInput playerInput;

        private InputAction _moveAction;
        private InputAction _moveHookAction;

        public float HookMoveInput { private set; get; }
        public Vector2 DiverMoveInput { private set; get; }
        
        public Subject<Unit> OnUseHook { private set; get; }
        public Subject<Unit> OnInteract { private set; get; }


        [Inject]
        void Inject(PlayerInput input)
        {
            if(playerInput == null && input != null)
                this.playerInput = input;
        }

        private void Awake()
        {
            OnUseHook = new Subject<Unit>();
            OnInteract = new Subject<Unit>();
        }

        private void Start()
        {
            InputAction interact=null, useHook=null, toggleSprint =null;
            if (useInputClassInstance)
            {
                var m_controls = new UnderTheSeaInput();
                throw new System.NotImplementedException();
            }
            else
            {
                _moveAction = GetActionSafe("Move");
                _moveHookAction = GetActionSafe("MoveHook");
                interact = GetActionSafe("Interact");
                useHook = GetActionSafe("Hook");
                toggleSprint = GetActionSafe("ToggleFastMove");
            }
          
            useHook.PerformedAsObservable().Subscribe(_ => OnUseHook.OnNext(Unit.Default));
            interact.PerformedAsObservable().Subscribe(_ => OnInteract.OnNext(Unit.Default));
            // useHook.PerformedAsObservable().Subscribe(t => { Debug.Log("Use Hook was performed"); });
            // interact.PerformedAsObservable().Subscribe(t => { Debug.Log("Interact was performed"); });

            OnUseHook.Subscribe(_ => Debug.Log("Use Hook was pressed"));
            OnInteract.Subscribe(_ => Debug.Log("Interact was pressed"));
        }

        private InputAction GetActionSafe(string moveHookName)
        {
            var moveHookAction = playerInput.actions[moveHookName];
            Debug.Assert(moveHookAction != null, $"No Action named {moveHookName} found!");
            return moveHookAction;
        }


        private void Update()
        {
            this.DiverMoveInput = _moveAction.ReadValue<Vector2>();
            this.HookMoveInput = _moveHookAction.ReadValue<float>();
            
        }
    }
}