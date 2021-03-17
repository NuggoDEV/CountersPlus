using BeatSaberMarkupLanguage.Components;
using CountersPlus.ConfigModels;
using CountersPlus.Custom;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.ViewControllers.Editing;
using CountersPlus.Utils;
using HMUI;
using Zenject;

namespace CountersPlus.UI.SettingGroups
{
    public class CountersSettingsGroup : SettingsGroup
    {
        [Inject] protected LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;
        [Inject] protected LazyInject<CountersPlusCounterEditViewController> editViewController;

        public override CustomListTableData.CustomCellInfo CellInfoForIdx(int idx)
        {
            var model = flowCoordinator.Value.AllConfigModels[idx];
            var customCellInfo = new CountersPlusListTableCell(idx, model.DisplayName);
            
            if (model is CustomConfigModel customConfig)
            {
                customCellInfo.text = customConfig.AttachedCustomCounter.Name;
                if (customConfig.AttachedCustomCounter.BSML != null && !string.IsNullOrEmpty(customConfig.AttachedCustomCounter.BSML.Icon))
                {
                    try
                    {
                        customCellInfo.icon = ImagesUtility.LoadSpriteFromExternalAssemblyResources(
                            customConfig.AttachedCustomCounter.CounterType.Assembly, customConfig.AttachedCustomCounter.BSML.Icon);
                    }
                    catch
                    {
                        customCellInfo.icon = LoadSprite("Counters.Custom");
                        customCellInfo.subtext = "\n<i>(Failed to load custom icon.)</i>";
                    }
                }
                else
                {
                    customCellInfo.icon = LoadSprite("Counters.Custom");
                }
            }
            else
            {
                customCellInfo.icon = LoadSprite($"Counters.{model.DisplayName}");
            }

            return customCellInfo;
        }

        public override int NumberOfCells() => flowCoordinator.Value.AllConfigModels.Count;

        public override int CellToSelect() => -1;

        public override void OnCellSelect(TableView view, int idx)
        {
            ConfigModel selectedModel = flowCoordinator.Value.AllConfigModels[idx];
            flowCoordinator.Value.SetRightViewController(editViewController.Value);
            editViewController.Value.ApplySettings(selectedModel);
        }

        public override void OnDisable() => flowCoordinator.Value.SetRightViewController(null);
    }
}
