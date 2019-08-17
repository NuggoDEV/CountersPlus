using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using IPA;
using IPA.Loader;
using IPALogger = IPA.Logging.Logger;
using CountersPlus.UI;
using CountersPlus.Harmony;

namespace CountersPlus
{
    public class Plugin : IBeatSaberPlugin
    {
        public static string PluginName => "CountersPlus";
        public static SemVer.Version PluginVersion { get; private set; } = new SemVer.Version("0.0.0"); // Default. Actual version number set on Init()

        internal static IPALogger Logger; //Conflicts with UnityEngine.Logger POG

        public enum LogInfo { Info, Warning, Notice, Error, Fatal };
        internal static BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config("CountersPlus"); //Conflicts with CountersPlus.Config POG
        internal static bool upToDate = true;
        internal static string webVersion;

        public void Init(IPALogger log, PluginLoader.PluginMetadata metadata)
        {
            Logger = log;
            Log("Logger prepared");

            if (metadata != null)
            {
                PluginVersion = metadata.Version;
            }
        }

        public void OnApplicationStart() => Load();
        public void OnApplicationQuit() { }

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

        private void Load(string msg = "started")
        {
            CountersPlusPatcehs.ApplyHarmonyPatches();
            CountersController.OnLoad();

            Log($"{PluginName} v.{PluginVersion} has {msg}.", LogInfo.Notice);
        }

        private void Unload()
        {
            CountersPlusPatcehs.RemoveHarmonyPatches();
        }

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
