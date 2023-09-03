using IPA;
using IPA.Config;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;
using IPA.Config.Stores;
using Timecoder.Configuration;
using Timecoder.Installers;
using Newtonsoft.Json;

namespace Timecoder
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    [NoEnableDisable]
    public class Plugin
    {
        public static string PluginVersion = "0.1.0";

        [Init]
        public Plugin(IPALogger logger, Config conf, Zenjector zenject)
        {
            zenject.UseLogger(logger);
            zenject.UseMetadataBinder<Plugin>();
            zenject.UseHttpService();

            zenject.Install<TCAppInstaller>(Location.App, conf.Generated<TCConfig>());
            zenject.Install<TCMenuInstaller>(Location.Menu);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            // Signal end session from here
            // Warning to myself: Unity main thread here!!!!!
        }
    }
}
