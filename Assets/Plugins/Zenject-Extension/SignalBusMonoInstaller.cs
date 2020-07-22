using UnityEngine;
using Zenject;

namespace Zenject
{
    public class SignalBusMonoInstaller : MonoInstaller<SignalBusMonoInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        }
    }
}