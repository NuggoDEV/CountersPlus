using CountersPlus.UI;
using HarmonyLib;
using IPA.Loader;
using IPA;
using System;
using System.Linq;
using IPALogger = IPA.Logging.Logger;
using CountersPlus.Utils;

namespace CountersPlus
{
    public enum LogInfo { Info, Warning, Notice, Error, Fatal };

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static SemVer.Version PluginVersion { get; private set; } = new SemVer.Version("0.0.0"); //Default.
        public static SemVer.Version WebVersion { get; internal set; } = new SemVer.Version("0.0.0"); //Default.

        internal static bool UpToDate => PluginVersion >= WebVersion;

        private static Harmony harmonyInstance;
        public const string harmonyId = "com.caeden117.beatsaber.countersplus";

        [Init]
        public void Init(IPALogger log, PluginMetadata metadata)
        {
            Logger.Init(log);   
            PluginVersion = metadata?.Version;
            BS_Utils.Utilities.BSEvents.gameSceneActive += GameCoreLoaded;
            BS_Utils.Utilities.BSEvents.menuSceneActive += ClearCounters;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            try
            {
                harmonyInstance = new Harmony(harmonyId);
                harmonyInstance.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Log($"{ex.Message}", LogInfo.Fatal, "Unable to apply Harmony patches. Did you even install BSIPA correctly?");
            }

            CountersController.OnLoad();
            MenuUI.CreateUI();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            if (harmonyInstance != null) harmonyInstance?.UnpatchAll(harmonyId);
            BS_Utils.Utilities.BSEvents.gameSceneActive -= GameCoreLoaded;
            BS_Utils.Utilities.BSEvents.menuSceneActive -= ClearCounters;
        }

        private void GameCoreLoaded()
        {
            PlayerDataModel dataModel = UnityEngine.Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault();
            if (CountersController.settings.Enabled && (dataModel?.playerData.playerSpecificSettings.noTextsAndHuds ?? true))
            {
                CountersController.LoadCounters();
            }
        }

        private void ClearCounters()
        {
            CountersController.LoadedCounters.Clear();
        }

        public static void Log(string m, LogInfo l = LogInfo.Info, string suggestedAction = null)
        {
            Logger.Log(m, l, suggestedAction);
        }
    }
}
