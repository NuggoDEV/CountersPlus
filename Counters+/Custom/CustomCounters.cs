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
        public static void CreateCustomCounter<T>(T model, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            Scene scene = SceneManager.GetActiveScene();
            foreach (SectionData section in data.Sections)
            {
                if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                    if (section.SectionName == model.Name) return;
            }
            if (scene.name == "" || scene.name == "Init" || scene.name == "EmptyTransition" || scene.name == "HealthWarning")
            {
                string modCreator = "";
                if (model.Mod != null)
                    modCreator = model.Mod.Name;
                if (model.BSIPAMod != null)
                {   //Fucking hell DaNike can't you expose the IBeatSaberPlugins so it would be easier for stuff like this!?!?
                    PluginLoader.PluginInfo info = PluginManager.AllPlugins.Where((PluginLoader.PluginInfo x) => model.BSIPAMod == x.GetPrivateField<IBeatSaberPlugin>("Plugin")).FirstOrDefault();
                    if (info != null)
                        modCreator = info.Metadata.Name;
                }
                CustomConfigModel counter = new CustomConfigModel(model.Name)
                {
                    DisplayName = model.Name,
                    SectionName = model.SectionName,
                    Enabled = true,
                    Position = ICounterPositions.BelowCombo,
                    Index = 2,
                    Counter = model.Counter,
                    ModCreator = modCreator,
                    RestrictedPositions = (restrictedPositions.Count() == 0 || restrictedPositions == null) ? new ICounterPositions[] { } : restrictedPositions, //Thanks Viscoci for this
                };
                if (string.IsNullOrEmpty(counter.SectionName) || string.IsNullOrEmpty(counter.DisplayName))
                    throw new CustomCounterException("Custom Counter properties invalid. Please make sure SectionName and DisplayName are properly assigned.");
                Plugin.Log($"Custom Counter added by: {modCreator}!", Plugin.LogInfo.Notice);
            }
            else
            {
                throw new CustomCounterException("It is too late to add Custom Counters. Please add Custom Counters at launch.");
            }
        }

        /// <summary>
        /// A simpler way to create a custom Counter.
        /// <param name="model"/>The CustomCounter object.</param>
        /// <param name="restrictedPositions">Restrict your Custom Counter to any of these positions. No parameters would allow the Counter to use all 6.</param>
        /// </summary>
        public static void Create<T>(T model, params ICounterPositions[] restrictedPositions) where T : CustomCounter
        {
            CreateCustomCounter(model, restrictedPositions);
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
        public string SectionName;
        public string Counter;
        public string ModCreator;
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