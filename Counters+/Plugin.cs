using CountersPlus.ConfigModels;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;
using CountersPlus.Installers;

namespace CountersPlus
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Logger { get; private set; }
        internal static MainConfigModel MainConfig { get; private set; }

        [Init]
        public Plugin(IPALogger logger,
            [Config.Name("CountersPlus")] Config conf)
        {
            Instance = this;
            Logger = logger;
            MainConfig = conf.Generated<MainConfigModel>();
        }

        [OnEnable]
        public void OnEnable()
        {
            Installer.RegisterAppInstaller<ConfigModelInstaller>();
            Installer.RegisterGameCoreInstaller<CountersInstaller>();
            Installer.RegisterMenuInstaller<MenuUIInstaller>();
        }

        [OnDisable]
        public void OnDisable()
        {
            Installer.UnregisterAppInstaller<ConfigModelInstaller>();
            Installer.UnregisterGameCoreInstaller<CountersInstaller>();
            Installer.UnregisterMenuInstaller<MenuUIInstaller>();
        }

    }
}
