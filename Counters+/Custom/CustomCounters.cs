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
        internal static List<CustomCounter> LoadedCustomCounters = new List<CustomCounter>();

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
            model.ModName = modCreator;

            Plugin.Log($"Custom Counter ({model.Name}) added!", LogInfo.Notice);

            List<CustomConfigModel> existingModels = ConfigLoader.LoadCustomCounters();

            model.ConfigModel.CustomCounter = model;
            if (restrictedPositions != null && restrictedPositions.Count() >= 1) model.RestrictedPositions = restrictedPositions;
            if (existingModels.Any(x => x.DisplayName == model.SectionName)) //It exists, wahoo!
                model.ConfigModel = existingModels.First(x => x.DisplayName == model.SectionName);
            else //This is a new counter!
            {
                model.ConfigModel.CustomCounter = model;
                model.IsNew = true;
                defaults.Save();
            }
            LoadedCustomCounters.Add(model);

        }
    }

    public class CustomCounter
    {
        /// <summary>
        /// The name in CountersPlus.ini that'll store variables. Try and keep to one name and not change it. It cannot conflict with other loaded counters.
        /// </summary>
        public string SectionName;
        /// <summary>
        /// The name of the counter. Will be shown in the submenu title.
        /// </summary>
        public string Name;
#pragma warning disable CS0618 // IPA is obsolete
        /// <summary>
        /// The plugin that created this custom counter. Will be displayed in the Settings UI.
        /// </summary>
        public IPlugin Mod;
#pragma warning restore CS0618 // IPA is obsolete
        /// <summary>
        /// The plugin that created this custom counter. Will be displayed in the Settings UI.
        /// </summary>
        public IBeatSaberPlugin BSIPAMod;
        /// <summary>
        /// The name of the GAmeObject that holds the Canvas of the counter.
        /// </summary>
        public string Counter;
        /// <summary>
        /// Description of the counter that will be put onto its cell in the Counters+ settings menu.
        /// </summary>
        public string Description;
        /// <summary>
        /// Namespace location to an icon that will be used to represent your counter in the settings menu.
        /// </summary>
        public string Icon_ResourceName;
        /// <summary>
        /// Positions that are restricted for this counter. By default, it is open to every position.
        /// </summary>
        public ICounterPositions[] RestrictedPositions = Enum.GetValues(typeof(ICounterPositions)) as ICounterPositions[];

        /// <summary>
        /// Local name of the mod from BSIPA or IPA.
        /// </summary>
        internal string ModName;
        /// <summary>
        /// Local config model.
        /// </summary>
        internal CustomConfigModel ConfigModel;
        /// <summary>
        /// Whether or not this Custom Counter has been newly added.
        /// </summary>
        internal bool IsNew = false;
    }

    public class CustomConfigModel : ConfigModel
    {
        public CustomConfigModel(CustomCounter counter) { DisplayName = counter.SectionName; }
        public CustomConfigModel(string name) { DisplayName = name; }
        public CustomConfigModel() { DisplayName = "NewCustomConfigModel"; }

        internal CustomCounter CustomCounter;
    }
}