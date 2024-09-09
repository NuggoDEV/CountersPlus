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
            if (model is CustomConfigModel customConfig)
            {
                if (customConfig.AttachedCustomCounter.BSML != null && !string.IsNullOrEmpty(customConfig.AttachedCustomCounter.BSML.Icon))
                {
                    try
                    {
                        return new CountersPlusListTableCell(idx, customConfig.AttachedCustomCounter.Name, null, ImagesUtility.LoadSpriteFromExternalAssemblyResources(
                            customConfig.AttachedCustomCounter.CounterType.Assembly, customConfig.AttachedCustomCounter.BSML.Icon));
                    }
                    catch
                    {
                        return new CountersPlusListTableCell(idx, customConfig.AttachedCustomCounter.Name,"\n<i>(Failed to load custom icon.)</i>", LoadSprite("Counters.Custom"));
                    }
                }
                else
                {
                    return new CountersPlusListTableCell(idx, customConfig.AttachedCustomCounter.Name, null, LoadSprite("Counters.Custom"));
                }
            }
            else
            {
                return new CountersPlusListTableCell(idx, model.DisplayName, null, LoadSprite($"Counters.{model.DisplayName}"));
            }
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
