using System;
using System.Reflection;
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
        /// <param name="model"/>The CustomCounter object.
        /// <param name="restrictedPositions"/>Restrict your Custom Counter to any of these positions. Inputting no parameters would allow the Counter to use all that are available.
        /// </summary>
        public static void Create<T>(T model, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            Create(model, null, Assembly.GetCallingAssembly(), restrictedPositions);
        }

        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system.
        /// <param name="model"/>The CustomCounter object.
        /// <param name="defaults"/>A <see cref="CustomConfigModel"/> that contains your default settings.
        /// </summary>
        public static void Create<T>(T model, CustomConfigModel defaults = null) where T : CustomCounter
        {
            Create(model, defaults, Assembly.GetCallingAssembly(), null);
        }

        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system.
        /// <param name="model"/>The CustomCounter object.
        /// <param name="defaults">Default configuration options for your custom counter.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions. Inputting no parameters would allow the Counter to use all that are available.</param>
        /// </summary>
        public static void Create<T>(T model, CustomConfigModel defaults = null, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            Create(model, defaults, Assembly.GetCallingAssembly(), restrictedPositions);
        }

        private static void Create<T>(T model, CustomConfigModel defaults, Assembly callingAssembly, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            string modCreator = "";
            if (model.Mod != null) modCreator = model.Mod.Name;

            if (model.BSIPAMod != null)
            {
                modCreator = model.BSIPAMod.Name;
            }
            model.ModName = modCreator;

            Plugin.Log($"Custom Counter ({model.Name}) added!", LogInfo.Notice);

            if (!string.IsNullOrEmpty(model.Icon_ResourceName))
                model.LoadedIcon = UIUtilities.LoadSpriteRaw(UIUtilities.GetResource(callingAssembly, model.Icon_ResourceName));

            List<CustomConfigModel> existingModels = ConfigLoader.LoadCustomCounters();

            if (restrictedPositions != null && restrictedPositions.Count() >= 1) model.RestrictedPositions = restrictedPositions;
            if (existingModels.Any(x => x.DisplayName == model.SectionName))
                model.ConfigModel = existingModels.First(x => x.DisplayName == model.SectionName);
            else //This is a new counter!
            {
                if (defaults is null)
                {
                    defaults = new CustomConfigModel(model);
                    defaults.Enabled = true;
                    defaults.Position = ICounterPositions.BelowCombo;
                    defaults.Distance = 2;
                }
                model.ConfigModel = defaults;
                model.IsNew = true;
                defaults.Save();
            }
            model.ConfigModel.CustomCounter = model;
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
        public IPA.Old.IPlugin Mod;
#pragma warning restore CS0618 // IPA is obsolete
        /// <summary>
        /// The plugin that created this custom counter. Will be displayed in the Settings UI.
        /// </summary>
        public PluginMetadata BSIPAMod;
        /// <summary>
        /// The name of the <see cref="GameObject"/> that holds the <see cref="Canvas"/> that contains all the text for the counter.
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
        /// Location to a BSML (*.bsml) file to load extra settings in the Counters+ menu.
        /// </summary>
        public string CustomSettingsResource = null;
        /// <summary>
        /// The <see cref="Type"/> of a <see cref="MonoBehaviour"/> that will handle the BSML file defined in <see cref="CustomSettingsResource"/>.
        /// </summary>
        public Type CustomSettingsHandler;
        /// <summary>
        /// An instance to a class that contains various attributes for a Counters+ template counter.
        /// </summary>
        public object TemplateCounter;

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
        /// <summary>
        /// Loaded icon from "Icon_ResourcesName".
        /// </summary>
        internal Sprite LoadedIcon = null;
    }

    public class CustomConfigModel : ConfigModel
    {
        public CustomConfigModel(CustomCounter counter) { DisplayName = counter.SectionName; }
        public CustomConfigModel(string name) { DisplayName = name; }
        public CustomConfigModel() { DisplayName = "NewCustomConfigModel"; }

        internal CustomCounter CustomCounter;
    }
}