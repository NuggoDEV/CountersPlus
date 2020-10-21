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
using HarmonyObj = HarmonyLib.Harmony;
using CountersPlus.Harmony;

namespace CountersPlus
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Logger { get; private set; }
        internal static MainConfigModel MainConfig { get; private set; }
        internal static Dictionary<Assembly, CustomCounter> LoadedCustomCounters { get; private set; } = new Dictionary<Assembly, CustomCounter>();

        private const string HARMONY_ID = "com.caeden117.countersplus";
        private HarmonyObj harmony;

        [Init]
        public Plugin(IPALogger logger,
            [Config.Name("CountersPlus")] Config conf,
            Zenjector zenjector)
        {
            Instance = this;
            Logger = logger;
            MainConfig = conf.Generated<MainConfigModel>();
            harmony = new HarmonyObj(HARMONY_ID);

            zenjector.OnApp<CoreInstaller>();
            zenjector.OnGame<CountersInstaller>()
                .Expose<CoreGameHUDController>()
                .ShortCircuitForTutorial()
                .ShortCircuitForMultiplayer(); // This is until I can safely confirm that C+ works with Multiplayer.
            zenjector.OnMenu<MenuUIInstaller>();
        }

        [OnEnable]
        public void OnEnable()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony.UnpatchAll(HARMONY_ID);
        }
    }
}
