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

        public override TableCell CellForIdx(TableView view, int idx)
        {
            GetCell(view, out var cell, out var packInfoText, out var packCoverImage);

            ConfigModel model = flowCoordinator.Value.AllConfigModels[idx];
            if (!(model is CustomConfigModel customConfig))
            {
                packInfoText.text = $"{model.DisplayName}";
                try
                {
                    packCoverImage.sprite = LoadSprite($"Counters.{model.DisplayName}");
                }
                catch { }
            }
            else
            {
                packInfoText.text = $"{customConfig.AttachedCustomCounter.Name}";
                if (customConfig.AttachedCustomCounter.BSML != null && !string.IsNullOrEmpty(customConfig.AttachedCustomCounter.BSML.Icon))
                {
                    try
                    {
                        packCoverImage.sprite = ImagesUtility.LoadSpriteFromExternalAssemblyResources(
                            customConfig.AttachedCustomCounter.CounterType.Assembly, customConfig.AttachedCustomCounter.BSML.Icon);
                    }
                    catch
                    {
                        packCoverImage.sprite = LoadSprite("Counters.Custom");
                        packInfoText.text += "\n<i>(Failed to load custom icon.)</i>";
                    }
                }
                else
                {
                    packCoverImage.sprite = LoadSprite("Counters.Custom");
                }
            }
            return cell;
        }

        public override int NumberOfCells() => flowCoordinator.Value.AllConfigModels.Count;

        public override int CellToSelect() => -1;

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
