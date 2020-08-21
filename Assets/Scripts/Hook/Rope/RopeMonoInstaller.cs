using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Hook.Rope
{
    public class RopeMonoInstaller : MonoInstaller<RopeMonoInstaller>
    {
        
        public override void InstallBindings()
        {
            Container.Bind<RopeTest>().FromComponentInHierarchy().AsSingle();
        }
    }
}