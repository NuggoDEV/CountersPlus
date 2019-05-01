using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountersPlus.Config;
using IPA.Loader;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.IO;
using IniParser.Model;
using IniParser;
using IPA;
using IPA.Old;

namespace CountersPlus.Custom
{
    public class CustomCounterCreator : MonoBehaviour
    {
        /// <summary>
        /// Adds an outside MonoBehaviour into the Counters+ system. If it already exists in the system, it will be ignored.
        /// <param name="model"/>The CustomCounter object.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions.</param>
        /// </summary>
        [Obsolete("Use the simpler CustomCounterCreator.Create() function for creating custom counters.")]
        public static void CreateCustomCounter<T>(T model, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            Create(model, null, restrictedPositions);
        }

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
        /// <param name="defaults">Default configuration options for your custom counter.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions. Inputting no parameters would allow the Counter to use all that are available.</param>
        /// </summary>
        public static void Create<T>(T model, CustomConfigModel defaults = null, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            Scene scene = SceneManager.GetActiveScene();

            string modCreator = "";
            if (model.Mod != null) modCreator = model.Mod.Name;
            if (model.BSIPAMod != null)
            {   //Fucking hell DaNike can't you expose the IBeatSaberPlugins so it would be easier for stuff like this!?!?
                PluginLoader.PluginInfo info = PluginManager.AllPlugins.Where((PluginLoader.PluginInfo x) => model.BSIPAMod == x.GetPrivateField<IBeatSaberPlugin>("Plugin")).FirstOrDefault();
                if (info != null) modCreator = info.Metadata.Name;
            }
            Plugin.Log($"Custom Counter added by: {modCreator}!", Plugin.LogInfo.Notice);

            foreach (SectionData section in data.Sections)
            {
                if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                    if (section.SectionName == model.Name) return;
            }

            if (!(scene.name == "" || scene.name == "Init" || scene.name == "EmptyTransition" || scene.name == "HealthWarning"))
                Plugin.Log("Custom Counter is being created after the recommended scenes. It might take a relaunch or a scene reload to appear!", Plugin.LogInfo.Warning);
            
            CustomConfigModel counter = new CustomConfigModel(model.Name)
            {
                DisplayName = model.Name,
                SectionName = model.SectionName,
                Enabled = (defaults == null ? true : defaults.Enabled),
                Position = (defaults == null ? ICounterPositions.BelowCombo : defaults.Position),
                Index = (defaults == null ? 2 : defaults.Index),
                Counter = model.Counter,
                ModCreator = modCreator,
                RestrictedPositions = (restrictedPositions.Count() == 0 || restrictedPositions == null) ? new ICounterPositions[] { } : restrictedPositions, //Thanks Viscoci for this
            };

            if (string.IsNullOrEmpty(counter.SectionName) || string.IsNullOrEmpty(counter.DisplayName))
                throw new CustomCounterException("Custom Counter properties invalid. Please make sure SectionName and DisplayName are properly assigned.");
        }
    }

    internal class CustomCounterException : Exception
    {
        public CustomCounterException(String msg) : base("Counters+ | " + msg) {
            Plugin.Log(msg, Plugin.LogInfo.Error, "Contact the developer of the infringing mod to check their Custom Counter creation code.");
        }
    }

    public class CustomCounter
    {
        [Obsolete("Counters+ no longer uses JSON for configuration. Consider using SectionName instead.")]
        public string JSONName { get { return SectionName; } set { SectionName = value; } }
        /// <summary>
        /// The name in CountersPlus.ini that'll store variables. Try and keep to one name and not change it. It cannot conflict with other loaded counters.
        /// </summary>
        public string SectionName { get; set; }
        /// <summary>
        /// The name of the counter. Will be shown in the submenu title.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The plugin that created this custom counter. Will be displayed in the Settings UI.
        /// </summary>
        #pragma warning disable CS0618 //Fuck off DaNike
        public IPlugin Mod { get; set; }
        /// <summary>
        /// The plugin that created this custom counter. Will be displayed in the Settings UI.
        /// </summary>
        public IBeatSaberPlugin BSIPAMod { get; set; }
        /// <summary>
        /// The name of the counter (as a GameObject) that will be added when it gets created.
        /// </summary>
        public string Counter { get; set; }
    }

    public class CustomConfigModel : IConfigModel
    {
        public CustomConfigModel(string name)
        {
            DisplayName = name;
        }
        internal string SectionName;
        internal string Counter;
        internal string ModCreator;
        internal ICounterPositions[] RestrictedPositions { get {
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