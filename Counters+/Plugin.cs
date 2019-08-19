using CountersPlus.UI;
using Harmony;
using IPA;
using IPA.Loader;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace CountersPlus
{
    public class Plugin : IBeatSaberPlugin
    {
        public static string PluginName => "CountersPlus";
        public static SemVer.Version PluginVersion { get; private set; } = new SemVer.Version("0.0.0"); // Default. Actual version number set on Init()

        internal static Plugin Instance;
        internal static IPALogger Logger; //Conflicts with UnityEngine.Logger POG

        public enum LogInfo { Info, Warning, Notice, Error, Fatal };
        internal static BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config("CountersPlus"); //Conflicts with CountersPlus.Config POG
        internal static bool upToDate = true;
        internal static string webVersion;

        private static HarmonyInstance harmonyInstance;
        public const string harmonyId = "com.caeden117.beatsaber.countersplus";

        public void Init(IPALogger log, PluginLoader.PluginMetadata metadata)
        {
            Logger = log;

            if (metadata != null)
            {
                PluginVersion = metadata.Version;
                Log("Version number set");
            }
        }

        public void OnApplicationStart()
        {
            Instance = this;

            try
            {
                harmonyInstance = HarmonyInstance.Create(harmonyId);
                harmonyInstance.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Log($"{ex.Message}\n{ex.StackTrace}", LogInfo.Fatal, "Unable to apply Harmony patches. Are you missing a dependency?");
            }

            CountersController.OnLoad();

            Log($"{PluginName} v.{PluginVersion} has started.", LogInfo.Notice);
        }

        public void OnApplicationQuit()
        {
            if (harmonyInstance != null)
            {
                harmonyInstance.UnpatchAll(harmonyId);
            }
        }

        public void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            //if (CountersController.settings.Enabled) CountersController.OnLoad();
            if (arg1.name == "GameCore" &&
                CountersController.settings.Enabled &&
                (!Resources.FindObjectsOfTypeAll<PlayerDataModelSO>()
                    .FirstOrDefault()?
                    .currentLocalPlayer.playerSpecificSettings.noTextsAndHuds ?? true)
                ) CountersController.LoadCounters();
        }

        public void OnSceneLoaded(Scene arg, LoadSceneMode hiBrian)
        {
            try
            {
                if (arg.name == "MenuCore") MenuUI.CreateUI();
            }catch(Exception e)
            {
                Log(e.ToString(), LogInfo.Fatal, "Install your dependencies!");
            }
        }

        public void OnSceneUnloaded(Scene scene) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }

        public static void Log(string m) => Log(m, LogInfo.Info);
        public static void Log(string m, LogInfo l) => Log(m, l, null);

        public static void Log(string m, LogInfo l, string suggestedAction)
        {
            IPALogger.Level level = IPALogger.Level.Debug;
            switch (l)
            {
                case LogInfo.Info: level = IPALogger.Level.Debug; break;
                case LogInfo.Notice: level = IPALogger.Level.Notice; break;
                case LogInfo.Warning: level = IPALogger.Level.Warning; break;
                case LogInfo.Error: level = IPALogger.Level.Error; break;
                case LogInfo.Fatal: level = IPALogger.Level.Critical; break;
            }
            Logger.Log(level, m);
            if (suggestedAction != null)
                Logger.Log(level, $"Suggested Action: {suggestedAction}");
        }
    }
}
