using System;
using System.Collections.Generic;
using System.Linq;
using CountersPlus.Config;
using CountersPlus.Custom;
using CountersPlus.Utils;
using HMUI;
using TMPro;
using UnityEngine;

namespace CountersPlus.UI.ViewControllers.SettingsGroups
{
    class CountersSettingsGroup : SettingsGroup
    {
        public List<SettingsInfo> counterInfos = new List<SettingsInfo>();
        public override SettingsGroupType type => SettingsGroupType.Counters;

        public override TableCell CellForIdx(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings)
        {
            if (!counterInfos.Any()) LoadData();
            LevelPackTableCell cell = view.DequeueReusableCellForIdentifier(settings.ReuseIdentifier) as LevelPackTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                cell = UnityEngine.Object.Instantiate(settings.levelPackTableCellInstance);
                cell.reuseIdentifier = settings.ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.
            TextMeshProUGUI packNameText = cell.GetPrivateField<TextMeshProUGUI>("_packNameText"); //Grab some TMP references.
            TextMeshProUGUI packInfoText = cell.GetPrivateField<TextMeshProUGUI>("_infoText");
            packInfoText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            UnityEngine.UI.Image packCoverImage = cell.GetPrivateField<UnityEngine.UI.Image>("_coverImage");
            packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images (especially transparent ones)

            SettingsInfo info = counterInfos[row];
            packNameText.text = info.Name;
            packInfoText.text = info.Description;
            try
            {
                if (info.IsCustom)
                {
                    if ((info.Model as CustomConfigModel).CustomCounter.LoadedIcon != null)
                        packCoverImage.sprite = (info.Model as CustomConfigModel).CustomCounter.LoadedIcon;
                    else packCoverImage.sprite = Images.Images.LoadSprite("Custom");
                    cell.showNewRibbon = (info.Model as CustomConfigModel).CustomCounter.IsNew;
                }
                else
                {
                    packCoverImage.sprite = Images.Images.LoadSprite(info.Name);
                    if (info.Model.VersionAdded != null)
                    {
                        cell.showNewRibbon = Plugin.PluginVersion == info.Model.VersionAdded;
                        packInfoText.text += $"\n\n<i>Version added: {info.Model.VersionAdded.ToString()}</i>";
                    }
                }
            }
            catch { packCoverImage.sprite = Images.Images.LoadSprite("Bug"); }
            return cell;
        }

        public override int NumberOfCells()
        {
            if (!counterInfos.Any()) LoadData();
            return counterInfos.Count;
        }

        public override void OnCellSelect(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings)
        {
            CountersPlusEditViewController.UpdateSettings(counterInfos[row].Model);
        }

        private void LoadData()
        {
            List<ConfigModel> loadedModels = TypesUtility.GetListOfType<ConfigModel>();
            loadedModels = loadedModels.Where(x => !(x is CustomConfigModel)).ToList();
            loadedModels.ForEach(x => x = ConfigLoader.DeserializeFromConfig(x, x.DisplayName) as ConfigModel);
            foreach (ConfigModel model in loadedModels) counterInfos.Add(CreateFromModel(model));
            foreach (CustomCounter potential in CustomCounterCreator.LoadedCustomCounters)
            {
                counterInfos.Add(new SettingsInfo()
                {
                    Name = potential.Name,
                    Description = string.IsNullOrEmpty(potential.Description) ? $"A custom counter added by {potential.ModName}!" : potential.Description,
                    Model = potential.ConfigModel,
                    IsCustom = true,
                });
            }
            counterInfos.RemoveAll(x => x is null);
        }

        private SettingsInfo CreateFromModel<T>(T settings) where T : ConfigModel //Counters+ stuff, OK to remove.
        {
            if (settings is CustomConfigModel) return null;
            SettingsInfo info = new SettingsInfo()
            {
                Name = settings.DisplayName,
                Description = DescriptionForModel(settings),
                Model = settings,
            };
            return info;
        }

        private string DescriptionForModel<T>(T settings) where T : ConfigModel //Counters+ stuff, OK to remove.
        {
            switch (settings.DisplayName)
            {   //Dont mind me just compressing some code.
                default: return "Huh, I dont know, I cant find this!";
                case "Missed": return "<i>MISS</i>";
                case "Notes": return "Notes hit over total notes!";
                case "Personal Best": return "<i>\"Did ya... miss me?\"</i>";
                case "Progress": return "The original you know and love.";
                case "Score": return "If the in-game score counter wasn't good enough.";
                case "Speed": return "<i>\"Speed, motherfucker, do you speak it?\"</i>";
                case "Cut": return "How well you hit those bloqs.";
                case "Spinometer": return "Can you break 3600 degrees per second?";
                case "Notes Left": return "<i>\"Ah shit, here we go again...\"</i>";
                case "Fail": return "You cannot hide from how trash you truly are...";
            }
        }
    }

    public class SettingsInfo
    {
        public string Name;
        public string Description;
        public ConfigModel Model;
        public bool IsCustom = false;
    }
}
