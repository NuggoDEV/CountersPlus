using CountersPlus.Custom;
using CountersPlus.ConfigModels;
using CountersPlus.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System.Collections.Generic;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace CountersPlus
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Logger { get; private set; }
        internal static MainConfigModel MainConfig { get; private set; }
        internal static Dictionary<Assembly, CustomCounter> LoadedCustomCounters { get; private set; } = new Dictionary<Assembly, CustomCounter>();

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
            Installer.RegisterAppInstaller<CoreInstaller>();
            Installer.RegisterGameCoreInstaller<CountersInstaller>();
            Installer.RegisterMenuInstaller<MenuUIInstaller>();
        }

        [OnDisable]
        public void OnDisable()
        {
            Installer.UnregisterAppInstaller<CoreInstaller>();
            Installer.UnregisterGameCoreInstaller<CountersInstaller>();
            Installer.UnregisterMenuInstaller<MenuUIInstaller>();
        }

    }
}
