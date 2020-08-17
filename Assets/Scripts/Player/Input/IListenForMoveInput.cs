using UnityEngine.InputSystem;

namespace Player
{
    public interface IListenForMoveInput
    {
        void OnMove(InputAction.CallbackContext context);
    }
}