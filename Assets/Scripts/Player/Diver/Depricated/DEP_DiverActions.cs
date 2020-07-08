using UnityEngine.InputSystem;

namespace Player.Diver
{
    [System.Obsolete("Use GameInput.DiverGameplayActions instead")]
    public class DEP_DiverActions
    {
        
        
        
        
        public DEP_DiverActions(UnderTheSeaInput.DiverGameplayActions diverGameplayActions)
        {
            MoveAction = diverGameplayActions.Move;
            ToggleFastMove = diverGameplayActions.ToggleFastMove;
            InteractAction = diverGameplayActions.Interact;
        }

        public InputAction MoveAction { get; }

        public InputAction ToggleFastMove { get; }

        public InputAction InteractAction { get; }

        
    }
}