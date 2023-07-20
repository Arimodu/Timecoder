using Zenject;

namespace Timecoder.Installers
{
    internal class TCMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TCEventManager>().AsSingle();
        }
    }
}
