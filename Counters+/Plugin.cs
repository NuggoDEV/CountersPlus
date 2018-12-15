using IllusionPlugin;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

namespace CountersPlus
{
    public class Plugin : IPlugin
    {
        public string Name => "Counters+";
        public string Version => "1.1.0";
        public static string beatSaberVersion { get; private set; }
        public enum LogInfo { Info, Warning, Error, Fatal };

        public static bool reloadConfig { get; private set; } = false;
        private static bool saveOnReload = false;

        public void OnApplicationStart()
        {
            if (File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + "/BeatSaberVersion.txt"))
            {   //I wont be specific (0.12.0p1 VS 0.12.0b1) unless those updates cause issues
                string version = File.ReadAllText(Environment.CurrentDirectory.Replace('\\', '/') + "/BeatSaberVersion.txt");
                if (version.Contains("0.12.0")) beatSaberVersion = "0.12.0";
                if (version.Contains("0.12.1")) beatSaberVersion = "0.12.1";
                Log("Found general Beat Saber version. Running: " + beatSaberVersion);
            }
            SceneManager.activeSceneChanged += SceneManager_sceneLoaded;
            SceneManager.sceneLoaded += addUI;
            CountersController.OnLoad();
        }

        public static void FlagConfigForReload(bool SaveOnReload = false)
        {
            reloadConfig = true;
            saveOnReload = SaveOnReload;
            Plugin.Log("Config flagged for reload!");
        }

        private void SceneManager_sceneLoaded(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GameCore" &&
                CountersController.settings.Enabled &&
                (!Resources.FindObjectsOfTypeAll<PlayerDataModelSO>()
                    .FirstOrDefault()?
                    .currentLocalPlayer.playerSpecificSettings.noTextsAndHuds ?? true)
                )
            {
                CountersController.LoadCounters();
            }
            if (reloadConfig)
            {
                if (saveOnReload) CountersController.settings.save();
                CountersController.settings = Config.Config.loadSettings();
                if (CountersController.Instance == null) CountersController.OnLoad();
                reloadConfig = false;
                saveOnReload = false;
            }
        }

        private void addUI(Scene arg, LoadSceneMode hiBrian)
        {
            try
            {
                if (arg.name == "Menu") CountersSettingsUI.CreateSettingsUI();
            }catch(Exception e)
            {
                Log(e.ToString(), LogInfo.Fatal);
            }
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManager_sceneLoaded;
            SceneManager.sceneLoaded -= addUI;
        }

        public void OnLevelWasLoaded(int level) { }
        public void OnLevelWasInitialized(int level) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }

        public static void Log(string m)
        {
            Log(m, LogInfo.Info);
        }

        public static void Log(string m, LogInfo l)
        {
            Console.WriteLine("Counters+ [" + l.ToString() + "] | " + m);
            if (l == LogInfo.Fatal)
            {
                Console.WriteLine("Counters+ [IMPORTANT] | Please go to #support in the Beat Saber Modding Group with this issue!");
            }
        }
    }
}
