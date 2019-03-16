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
using IniParser.Model;
using IniParser;

namespace CountersPlus.Custom
{
    public class CustomCounterCreator : MonoBehaviour
    {
        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system. If it already exists in the system, it will be ignored.
        /// <param name="model"/>The CustomCounter object.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions.</param>
        /// </summary>
        public static void CreateCustomCounter<T>(T model, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            Scene scene = SceneManager.GetActiveScene();
            foreach (SectionData section in data.Sections)
            {
                if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                {
                    if (section.SectionName == model.Name) return;
                }
            }
            if (scene.name == "" || scene.name == "Init" || scene.name == "EmptyTransition" || scene.name == "HealthWarning")
            {
                CustomConfigModel counter = new CustomConfigModel(model.Name)
                {
                    DisplayName = model.Name,
                    SectionName = model.JSONName,
                    Enabled = true,
                    Position = ICounterPositions.BelowCombo,
                    Index = 2,
                    Counter = model.Counter,
                    ModCreator = model.Mod.Name,
                    RestrictedPositions = (restrictedPositions.Count() == 0 || restrictedPositions == null) ? new ICounterPositions[] { } : restrictedPositions, //Thanks Viscoci for this
                };
                if (string.IsNullOrEmpty(counter.SectionName) || string.IsNullOrEmpty(counter.DisplayName))
                    throw new CustomCounterException("Custom Counter properties invalid. Please make sure JSONName and DisplayName are properly assigned.");
                Plugin.Log("Custom Counter added!");
            }
            else
            {
                throw new CustomCounterException("It is too late to add Custom Counters. Please add Custom Counters at launch.");
            }
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
        public CustomConfigModel(string name)
        {
            DisplayName = name;
        }
        [Obsolete("Custom Counters is no longer stored using JSON. Consider using \"SectionName\" instead.")]
        public string JSONName { get {
                return SectionName;
            } set {
                SectionName = value;
            } }
        public string SectionName { get {
                return Plugin.config.GetString(DisplayName, "SectionName", "unknown", false);
            } set {
                Plugin.config.SetString(DisplayName, "SectionName", value);
            }
        }
        public string Counter
        {
            get
            {
                return Plugin.config.GetString(DisplayName, "Counter", "unknown", false);
            }
            set
            {
                Plugin.config.SetString(DisplayName, "Counter", value);
            }
        }
        public string ModCreator {
            get
            {
                return Plugin.config.GetString(DisplayName, "ModCreator", "unknown", false);
            }
            set
            {
                Plugin.config.SetString(DisplayName, "ModCreator", value);
            }
        }
        public ICounterPositions[] RestrictedPositions { get {
                string doodads = Plugin.config.GetString(DisplayName, "RestrictedPositions", "All", true);
                if (doodads == "All") return new ICounterPositions[] { };
                string[] split = doodads.Split(',');
                List<ICounterPositions> restricted = new List<ICounterPositions>();
                foreach(string position in split)
                {
                    restricted.Add((ICounterPositions)Enum.Parse(typeof(ICounterPositions), position));
                }
                return restricted.ToArray();
            } set {
                try
                {
                    string combined = string.Join(",", value);
                    if (combined.Length == 0) combined = "All";
                    Plugin.config.SetString(DisplayName, "RestrictedPositions", combined);
                }
                catch {
                    Plugin.config.SetString(DisplayName, "RestrictedPositions", "All");
                }
            } }
    }
}