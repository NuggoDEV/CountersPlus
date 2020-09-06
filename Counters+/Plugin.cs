using CountersPlus.ConfigModels;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;
using CountersPlus.Installers;
using CountersPlus.Custom;
using System.Collections.Generic;
using System.Reflection;
using HarmonyObj = HarmonyLib.Harmony;

namespace CountersPlus
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Logger { get; private set; }
        internal static MainConfigModel MainConfig { get; private set; }
        internal static Dictionary<Assembly, CustomCounter> LoadedCustomCounters { get; private set; } = new Dictionary<Assembly, CustomCounter>();

        private const string harmonyID = "com.caeden117.countersplus";

        private static HarmonyObj harmony;

        [Init]
        public Plugin(IPALogger logger,
            [Config.Name("CountersPlus")] Config conf)
        {
            Instance = this;
            Logger = logger;
            harmony = new HarmonyObj(harmonyID);
            MainConfig = conf.Generated<MainConfigModel>();
        }

        [OnEnable]
        public void OnEnable()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Installer.RegisterAppInstaller<ConfigModelInstaller>();
            Installer.RegisterGameCoreInstaller<CountersInstaller>();
            Installer.RegisterMenuInstaller<MenuUIInstaller>();
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony.UnpatchAll();
            Installer.UnregisterAppInstaller<ConfigModelInstaller>();
            Installer.UnregisterGameCoreInstaller<CountersInstaller>();
            Installer.UnregisterMenuInstaller<MenuUIInstaller>();
        }

    }
}
