using CountersPlus.ConfigModels;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.ViewControllers.Editing;
using HMUI;
using Zenject;

namespace CountersPlus.UI.SettingGroups
{
    public class CountersSettingsGroup : SettingsGroup
    {
        [Inject] protected LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;
        [Inject] protected LazyInject<CountersPlusCounterEditViewController> editViewController;

        public override TableCell CellForIdx(TableView view, int idx)
        {
            GetCell(view, out var cell, out var packInfoText, out var packCoverImage);

            ConfigModel model = flowCoordinator.Value.AllConfigModels[idx];
            packInfoText.text = $"{model.DisplayName}";
            try
            {
                packCoverImage.sprite = LoadSprite($"Counters.{model.DisplayName}");
            }
            catch { }
            return cell;
        }

        public override int NumberOfCells() => flowCoordinator.Value.AllConfigModels.Count;

        public override void OnCellSelect(TableView view, int idx)
        {
            ConfigModel selectedModel = flowCoordinator.Value.AllConfigModels[idx];
            flowCoordinator.Value.SetRightViewController(editViewController.Value);
            editViewController.Value.ApplySettings(selectedModel);
        }

        public override void OnDisable()
        {
            flowCoordinator.Value.SetRightViewController(null);
        }
    }
}
