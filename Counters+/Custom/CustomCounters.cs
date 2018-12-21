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

namespace CountersPlus.Custom
{
    public class CustomCounterCreator
    {
        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system. If it already exists in the system, it will be ignored.
        /// </summary>
        public static void CreateCustomCounter<T>(T model) where T : CustomCounter
        {
            if (CountersController.settings.CustomCounters.Where((CustomConfigModel x) => x.JSONName == model.JSONName).Count() > 0)
            {
                Plugin.Log("Attempted custom counter already exists in the system. Ignoring...", Plugin.LogInfo.Warning);
            }
            else
            {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "" || scene.name == "Init" || scene.name == "EmptyTransition" || scene.name == "HealthWarning")
                {
                    CustomConfigModel counter = new CustomConfigModel
                    {
                        JSONName = model.JSONName,
                        DisplayName = model.Mod.Name,
                    };
                    if (string.IsNullOrEmpty(counter.JSONName) || string.IsNullOrEmpty(counter.DisplayName))
                        throw new CustomCounterException("Custom Counter properties invalid. Please make sure JSONName and DisplayName are properly assigned.");
                    CountersController.settings.CustomCounters.Add(counter);
                    CountersController.customCounters.Add(model);
                    Plugin.Log("Custom Counter successfully added!");
                    Plugin.FlagConfigForReload(true);
                }
                else
                {
                    throw new CustomCounterException("It is too late to add Custom Counters. Please add Custom Counters at launch.");
                }
            }
        }
    }

    internal class CustomCounterException : Exception
    {
        public CustomCounterException(String msg) : base("Counters+ | " + msg) { }
    }

    public class CustomCounter
    {
        public CustomCounter() { }

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
        /// The counter that will be added when it gets created.
        /// </summary>
        public MonoBehaviour Counter { get; set; }
        /// <summary>
        /// A function that will be called when the UI element is created which will allow more advanced options to be displayed.
        /// </summary>
        public Action<SubMenu> AdvancedOptions { get; set; }
    }

    public class CustomConfigModel : IConfigModel
    {
        public string JSONName { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
    }
}