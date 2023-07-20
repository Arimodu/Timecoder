using Timecoder.Configuration;
using Timecoder.Network;
using Zenject;

namespace Timecoder.Installers
{
    internal class TCAppInstaller : Installer
    {
        private TCConfig Config { get; }

        internal TCAppInstaller(TCConfig config) 
        { 
            Config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(Config);
            Container.BindInterfacesAndSelfTo<TCClient>().AsSingle();
        }
    }
}
