using Zenject;

namespace Diver
{
    public class DiverInstaller : MonoInstaller<DiverInstaller>
    {
        public override void InstallBindings()
        {
            
            Container.Bind<DiverInput>().FromComponentInHierarchy().AsSingle();
        }
    }
}