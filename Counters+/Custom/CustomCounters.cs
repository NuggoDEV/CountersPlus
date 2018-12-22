using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountersPlus.Config;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using CustomUI.Settings;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace CountersPlus.Custom
{
    public class CustomCounterCreator : MonoBehaviour
    {
        static CustomCounterCreator Instance;
        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system. If it already exists in the system, it will be ignored.
        /// <param name="model"/>The CustomCounter object.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions.</param>
        /// </summary>
        public static void CreateCustomCounter<T>(T model, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            try
            {
                if (File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters/{model.JSONName}.json"))
                {
                    Plugin.Log("Attempted custom counter already exists in the system. Ignoring...", Plugin.LogInfo.Warning);
                    return;
                }
            }
            catch { }
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "" || scene.name == "Init" || scene.name == "EmptyTransition" || scene.name == "HealthWarning")
            {
                CustomConfigModel counter = new CustomConfigModel
                {
                    JSONName = model.JSONName,
                    DisplayName = model.Name,
                    Enabled = true,
                    Position = ICounterPositions.BelowCombo,
                    Index = 2,
                    Counter = model.Counter,
                    ModCreator = model.Mod.Name,
                    RestrictedPositions = (restrictedPositions.Count() == 0 || restrictedPositions == null) ? null : restrictedPositions,
                };
                if (string.IsNullOrEmpty(counter.JSONName) || string.IsNullOrEmpty(counter.DisplayName))
                    throw new CustomCounterException("Custom Counter properties invalid. Please make sure JSONName and DisplayName are properly assigned.");
                EnsureSettingsExist(counter);
            }
            else
            {
                throw new CustomCounterException("It is too late to add Custom Counters. Please add Custom Counters at launch.");
            }
        }

        internal static Task EnsureSettingsExist(CustomConfigModel counter)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (CountersController.settings != null)
                        {
                            add(counter);
                            Plugin.Log("Settings must exist now!");
                            break;
                        }
                    }
                    catch(Exception e) { Plugin.Log(e.ToString()); }
                    Thread.Sleep(10);
                }
            });
        }

        internal static void add(CustomConfigModel counter)
        {
            //CountersController.settings.CustomCounters.Add(counter);
            using (StreamWriter file = File.CreateText(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters/{counter.JSONName}.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() => {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.Formatting = Formatting.Indented;
                    return settings;
                });
                serializer.Serialize(file, counter);
            }
            Plugin.Log("Custom Counter successfully added!");
        }
    }

    internal class CustomCounterException : Exception
    {
        public CustomCounterException(String msg) : base("Counters+ | " + msg) { }
    }

    public class CustomCounter
    {

        /// <summary>
        /// The name in JSON that'll store variables. Try and keep to one name and not change it. It cannot conflict with other loaded counters.
        /// </summary>
        public string JSONName { get; set; }
        /// <summary>
        /// The name of the counter. Will be shown in the submenu title.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Whether or not a custom settings menu will be created for this counter.
        /// </summary>
        public bool CreateSettingsUI { get; set; }
        /// <summary>
        /// The plugin that created this custom counter. Will be displayed in the Settings UI.
        /// </summary>
        public IPlugin Mod { get; set; }
        /// <summary>
        /// The name of the counter that will be added when it gets created.
        /// </summary>
        public string Counter { get; set; }
    }

    public class CustomConfigModel : IConfigModel
    {
        public string JSONName { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
        public string Counter { get; set; }
        public string ModCreator { get; set; }
        public ICounterPositions[] RestrictedPositions { get; set; }
    }
}