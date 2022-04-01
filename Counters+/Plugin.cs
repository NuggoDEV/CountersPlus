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

namespace CountersPlus
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Logger { get; private set; }
        internal static MainConfigModel MainConfig { get; private set; }
        internal static List<CustomCounter> LoadedCustomCounters { get; private set; } = new List<CustomCounter>();

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

            zenjector.Expose<CoreGameHUDController>("Environment");

            zenjector.Install<CoreInstaller>(Location.App);
            zenjector.Install<MenuUIInstaller>(Location.Menu);

            zenjector.Install<CountersInstaller>(Location.StandardPlayer | Location.CampaignPlayer);
            zenjector.Install<MultiplayerCountersInstaller, MultiplayerLocalActivePlayerInstaller>();
        }

        [OnEnable]
        public void OnEnable() => harmony.PatchAll(Assembly.GetExecutingAssembly());

        [OnDisable]
        public void OnDisable() => harmony.UnpatchSelf();
    }
}
