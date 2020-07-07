namespace Core
{
    public static class GameInput
    {
        public static UnderTheSeaInput.HookActions HookActions { get; }
        public static UnderTheSeaInput.DiverGameplayActions DiverGameplayActions { get; }
        public static UnderTheSeaInput.UIActions UiActions { get; }

        static GameInput()
        {
            var gameInput = new UnderTheSeaInput();
            HookActions = gameInput.Hook;
            DiverGameplayActions = gameInput.DiverGameplay;
            UiActions = gameInput.UI;
        }
    }
}