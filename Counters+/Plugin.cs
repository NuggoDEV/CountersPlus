using IllusionPlugin;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

namespace CountersPlus
{
    public class Plugin : IPlugin
    {
        public string Name => "Counters+";
        public string Version => "1.0.0";
        public enum LogInfo { Info, Warning, Error, Fatal };

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManager_sceneLoaded;
            SceneManager.sceneLoaded += addUI;
            CountersController.OnLoad();
        }

        private void SceneManager_sceneLoaded(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GameCore") CountersController.LoadCounters();
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
