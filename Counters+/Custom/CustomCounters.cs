using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IPA;
using IPA.Loader;
using IPA.Old;
using CountersPlus.Config;
using CountersPlus.Utils;

namespace CountersPlus.Custom
{
    public class CustomCounterCreator : MonoBehaviour
    {
        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system.
        /// <param name="model"/>The CustomCounter object.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions. Inputting no parameters would allow the Counter to use all that are available.</param>
        /// </summary>
        public static void Create<T>(T model, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            Create(model, null, restrictedPositions);
        }

        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system.
        /// <param name="model"/>The CustomCounter object.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions. Inputting no parameters would allow the Counter to use all that are available.</param>
        /// </summary>
        public static void Create<T>(T model, CustomConfigModel defaults = null) where T : CustomCounter
        {
            Create(model, defaults, null);
        }

        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system.
        /// <param name="model"/>The CustomCounter object.</param>
        /// <param name="defaults">Default configuration options for your custom counter.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions. Inputting no parameters would allow the Counter to use all that are available.</param>
        /// </summary>
        public static void Create<T>(T model, CustomConfigModel defaults = null, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            string modCreator = "";
            if (model.Mod != null) modCreator = model.Mod.Name;

            if (model.BSIPAMod != null)
            {
                PluginLoader.PluginMetadata pluginMetadata = PluginUtility.GetPluginMetadata(model.BSIPAMod);
                if (pluginMetadata != null) modCreator = pluginMetadata.Name;
            }

            Plugin.Log($"Custom Counter ({model.Name}) added!", LogInfo.Notice);

            foreach (CustomConfigModel potential in ConfigLoader.LoadCustomCounters())
            {
                if (potential.DisplayName == model.Name) {
                    if (potential.IsNew)
                    {
                        potential.IsNew = false;
                        potential.Save();
                    }
                    return;
                }
            }

            CustomConfigModel counter = new CustomConfigModel(model.Name)
            {
                DisplayName = model.Name,
                SectionName = model.SectionName,
                Enabled = (defaults == null ? true : defaults.Enabled),
                Position = (defaults == null ? ICounterPositions.BelowCombo : defaults.Position),
                Distance = (defaults == null ? 2 : defaults.Distance),
                Counter = model.Counter,
                ModCreator = modCreator,
                IsNew = true,
                RestrictedPositions = (restrictedPositions?.Count() == 0 || restrictedPositions == null) ? new ICounterPositions[] { } : restrictedPositions, //Thanks Viscoci for this
            };

            if (string.IsNullOrEmpty(counter.SectionName) || string.IsNullOrEmpty(counter.DisplayName))
                throw new CustomCounterException("Custom Counter properties invalid. Please make sure SectionName and DisplayName are properly assigned.");

            counter.Save();
        }
    }

    internal class CustomCounterException : Exception
    {
        public CustomCounterException(String msg) : base("Counters+ | " + msg) {
            Plugin.Log(msg, LogInfo.Error, "Contact the developer of the infringing mod to check their Custom Counter creation code.");
        }
    }

    public class CustomCounter
    {
        /// <summary>
        /// The name in CountersPlus.ini that'll store variables. Try and keep to one name and not change it. It cannot conflict with other loaded counters.
        /// </summary>
        public string SectionName { get; set; }
        /// <summary>
        /// The name of the counter. Will be shown in the submenu title.
        /// </summary>
        public string Name { get; set; }
        #pragma warning disable CS0618 // IPA is obsolete
        /// <summary>
        /// The plugin that created this custom counter. Will be displayed in the Settings UI.
        /// </summary>
        public IPlugin Mod { get; set; }
        #pragma warning restore CS0618 // IPA is obsolete
        /// <summary>
        /// The plugin that created this custom counter. Will be displayed in the Settings UI.
        /// </summary>
        public IBeatSaberPlugin BSIPAMod { get; set; }
        /// <summary>
        /// The name of the counter (as a GameObject) that will be added when it gets created.
        /// </summary>
        public string Counter { get; set; }
    }

    public class CustomConfigModel : ConfigModel
    {
        public CustomConfigModel(string name)
        {
            DisplayName = name;
        }
        internal string SectionName;
        internal string Counter;
        internal string ModCreator;
        internal bool IsNew = false;
        internal ICounterPositions[] RestrictedPositions { get {
                string doodads = Plugin.config.GetString(DisplayName, "RestrictedPositions", "All", true);
                if (doodads == "All") return new ICounterPositions[] { };
                List<ICounterPositions> restricted = new List<ICounterPositions>();
                foreach(string position in doodads.Split(','))
                    restricted.Add((ICounterPositions)Enum.Parse(typeof(ICounterPositions), position));
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