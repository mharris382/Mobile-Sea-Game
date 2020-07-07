using UnityEngine.InputSystem;

namespace Player.Diver
{
    public class DiverActions
    {
        public DiverActions(InputAction moveAction, InputAction toggleFastMove, InputAction interactAction)
        {
            MoveAction = moveAction;
            ToggleFastMove = toggleFastMove;
            InteractAction = interactAction;
        }

        public InputAction MoveAction { get; }

        public InputAction ToggleFastMove { get; }

        public InputAction InteractAction { get; }
    }
}