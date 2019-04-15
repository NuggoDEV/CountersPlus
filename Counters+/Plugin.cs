using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using CountersPlus.UI;
using IPA;
using IPA.Logging;

namespace CountersPlus
{
    public class Plugin : IBeatSaberPlugin
    {
        internal static Plugin Instance;
        internal static IPA.Logging.Logger Logger; //Conflicts with UnityEngine.Logger POG
        public enum LogInfo { Info, Warning, Notice, Error, Fatal };
        internal static BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config("CountersPlus"); //Conflicts with CountersPlus.Config POG
        internal static bool upToDate = true;
        internal static string webVersion;

        public void Init(object thisIsNull, IPA.Logging.Logger log)
        {
            Logger = log;
            VersionChecker.GetOnlineVersion();
            Instance = this;
            if (!File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini"))
                File.Create(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            CountersController.OnLoad();
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

        public void OnApplicationStart() { }
        public void OnApplicationQuit() { }
        public void OnSceneUnloaded(Scene scene) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }

        public static void Log(string m)
        {
            Log(m, LogInfo.Info);
        }

        public static void Log(string m, LogInfo l)
        {
            Log(m, l, null);
        }

        public static void Log(string m, LogInfo l, string suggestedAction)
        {
            IPA.Logging.Logger.Level level = IPA.Logging.Logger.Level.Debug;
            switch (l)
            {
                case LogInfo.Info: level = IPA.Logging.Logger.Level.Debug; break;
                case LogInfo.Notice: level = IPA.Logging.Logger.Level.Notice; break;
                case LogInfo.Warning: level = IPA.Logging.Logger.Level.Warning; break;
                case LogInfo.Error: level = IPA.Logging.Logger.Level.Error; break;
                case LogInfo.Fatal: level = IPA.Logging.Logger.Level.Critical; break;
            }
            Logger.Log(level, m);
            if (suggestedAction != null)
                Logger.Log(level, $"Suggested Action: {suggestedAction}");
        }
    }
}
