using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BeatSaberMarkupLanguage;
using CountersPlus.Config;
using System.Reflection;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Attributes;
using System.Collections;

namespace CountersPlus.UI.ViewControllers.ConfigModelControllers
{
    public class ConfigModelController : MonoBehaviour
    {
        private string baseConfigLocation => Utilities.GetResourceContent(Assembly.GetAssembly(GetType()),
            "CountersPlus.UI.BSML.SettingsBase.bsml");

        public string Content => Utilities.GetResourceContent(Assembly.GetAssembly(GetType()),
            $"CountersPlus.UI.BSML.Config.{string.Join("", ConfigModel?.DisplayName.Split(' ')) ?? "Error"}.bsml");
        public virtual bool UseBaseSettings { get; set; } = true;
        public ConfigModel ConfigModel = null;
        public Component ModelSpecificController = null;

        [UIValue("enabled_value")]
        public bool Enabled { get => ConfigModel.Enabled; set => ConfigModel.Enabled = value; }

        [UIValue("position_value")]
        public ICounterPositions Position { get => ConfigModel.Position; set => ConfigModel.Position = value; }
        [UIValue("position_options")]
        public List<object> PosOptions => AdvancedCounterSettings.Positions.Keys.Cast<object>().ToList();
        [UIAction("position_formatter")]
        public string Format(ICounterPositions pos) => AdvancedCounterSettings.Positions[pos];

        [UIValue("distance_value")]
        public int Distance { get => ConfigModel.Distance; set => ConfigModel.Distance = value; }
        [UIValue("distance_options")]
        public List<object> DistanceOptions => AdvancedCounterSettings.Distances.Cast<object>().ToList();

        private GameObject editControllerBase;

        public static ConfigModelController GenerateController(ConfigModel model, Type controllerType, GameObject baseTransform)
        {
            GameObject controllerGO = new GameObject($"Counters+ | {model.DisplayName} Settings Controller");
            controllerGO.transform.parent = baseTransform.transform;
            ConfigModelController controller = controllerGO.AddComponent<ConfigModelController>();
            controller.UseBaseSettings = true;
            controller.ConfigModel = model;
            controller.editControllerBase = baseTransform;
            if (controllerType != null)
            {
                controller.ModelSpecificController = controllerGO.AddComponent(controllerType);
                controller.ModelSpecificController.SetPrivateField("parentController", controller);
            }
            controller.Apply();
            return controller;
        }

        public static ConfigModelController GenerateController(Type controllerType, GameObject baseTransform,
            string bsmlFileName = null, bool addBaseSettings = false, ConfigModel model = null)
        {
            string name = bsmlFileName == null ? "Blank" : bsmlFileName;
            GameObject controllerGO = new GameObject($"Counters+ | {name} Settings Controller");
            controllerGO.transform.parent = baseTransform.transform;
            ConfigModelController controller = controllerGO.AddComponent<ConfigModelController>();
            controller.ConfigModel = model;
            if (controllerType != null)
                controller.ModelSpecificController = controllerGO.AddComponent(controllerType);
            if (addBaseSettings)
                BSMLParser.instance.Parse(controller.baseConfigLocation, baseTransform, controller);
            try
            {
                if (bsmlFileName != null)
                    BSMLParser.instance.Parse(Utilities.GetResourceContent(controllerType.Assembly,
                        bsmlFileName), baseTransform, controller.ModelSpecificController);
            }catch(Exception e)
            {
                Plugin.Log($"Exception thrown while attempting to load a non-Counters+ BSML file:\n{bsmlFileName}\n{e}",
                    LogInfo.Error,
                    "Report this issue to the mod author who created this Custom Counter, rather than to Counters+ itself.");
            }
            return controller;
        }

        private void Apply()
        {
            if (UseBaseSettings && ConfigModel != null)
                BSMLParser.instance.Parse(baseConfigLocation, editControllerBase, this);
            if ((ConfigModel != null || !UseBaseSettings) && ModelSpecificController != null)
                BSMLParser.instance.Parse(Content, editControllerBase, ModelSpecificController);
            else if (ConfigModel is null) Plugin.Log("ConfigModel does not exist!", LogInfo.Warning);
        }

        private void OnDestroy()
        {
            ConfigModel?.Save();
        }

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            StartCoroutine(DelayedMockCounterUpdate(ConfigModel));
        }

        private IEnumerator DelayedMockCounterUpdate<T>(T settings) where T : ConfigModel
        {
            yield return new WaitForEndOfFrame();
            settings?.Save();
            MockCounter.Update(settings);
        }
    }
}
