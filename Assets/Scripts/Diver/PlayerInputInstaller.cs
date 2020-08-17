using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using Zenject;

namespace Diver
{
    public class PlayerInputInstaller : MonoInstaller<PlayerInputInstaller>
    {
        [Required, SceneObjectsOnly] public PlayerInput playerInput;
        
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerInput>().FromInstance(playerInput).AsSingle();
        }
    }
}