using UnityEngine;

namespace Core
{
    public static class GameInput
    {
        public static UnderTheSeaInput AllInputsInput { get; }
        public static UnderTheSeaInput.HookActions HookActions { get; }
        public static UnderTheSeaInput.DiverGameplayActions DiverGameplayActions { get; }
        public static UnderTheSeaInput.UIActions UiActions { get; }

        static GameInput()
        {
            Core.GameInput.AllInputsInput = new UnderTheSeaInput();
            AllInputsInput.Enable();
            HookActions = AllInputsInput.Hook;
            DiverGameplayActions = AllInputsInput.DiverGameplay;
            UiActions = AllInputsInput.UI;
            DiverGameplayActions.Enable();
            Debug.Log("Enabling DiverGameplayActions".InBold());
        }
        
        
        
        
    }
}