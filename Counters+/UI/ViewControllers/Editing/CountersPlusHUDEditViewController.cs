using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using CountersPlus.ConfigModels;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.Utils;
using HMUI;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace CountersPlus.UI.ViewControllers.Editing
{
    public class CountersPlusHUDEditViewController : ViewController
    {
        private readonly string SettingsBase = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "CountersPlus.UI.BSML.HUDs.HUDEdit.bsml");

        [Inject] private LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;
        [Inject] private LazyInject<CountersPlusHUDListViewController> hudList;
        [Inject] private CanvasUtility canvasUtility;
        [Inject] private MainConfigModel mainConfig;
        [Inject] private HUDConfigModel hudConfig;

        private int canvasID = -1;
        private HUDCanvas currentlyEditing = null;

        public void ApplyCanvasForEditing(int id)
        {
            if (id != canvasID)
            {
                hudList.Value.RefreshData();
            }
            canvasID = id;
            HUDCanvas settings = canvasUtility.GetCanvasSettingsFromID(canvasID);
            if (currentlyEditing != null)
            {
                currentlyEditing.OnCanvasSettingsChanged -= CurrentlyEditing_OnCanvasSettingsChanged;
                currentlyEditing.OnCanvasSettingsApply -= CurrentlyEditing_OnCanvasSettingsApply;
            }
            ClearScreen();
            var param = BSMLParser.Instance.Parse(SettingsBase, gameObject, settings);
            currentlyEditing = settings;
            currentlyEditing.OnCanvasSettingsChanged += CurrentlyEditing_OnCanvasSettingsChanged;
            currentlyEditing.OnCanvasSettingsApply += CurrentlyEditing_OnCanvasSettingsApply;
            StringSetting nameFieldSetting = param.GetObjectsWithTag("name-field").First().GetComponent<StringSetting>();
            nameFieldSetting.Interactable = !(currentlyEditing?.IsMainCanvas ?? true);
        }

        private void CurrentlyEditing_OnCanvasSettingsApply()
        {
            flowCoordinator.Value.SetRightViewController(null);
            hudList.Value.ClearSelection();
            hudList.Value.RefreshData();
        }

        private void CurrentlyEditing_OnCanvasSettingsChanged()
        {
            canvasUtility.UnregisterCanvas(canvasID);
            canvasUtility.CreateCanvasWithConfig(currentlyEditing);
            if (currentlyEditing.IsMainCanvas)
            {
                hudConfig.MainCanvasSettings = currentlyEditing;
            }
            else
            {
                hudConfig.OtherCanvasSettings.RemoveAt(canvasID);
                hudConfig.OtherCanvasSettings.Insert(canvasID, currentlyEditing);
            }
            mainConfig.HUDConfig = hudConfig;
            canvasUtility.RegisterNewCanvas(currentlyEditing, canvasID);
            flowCoordinator.Value.RefreshAllMockCounters();
        }

        private void ClearScreen()
        {
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}
