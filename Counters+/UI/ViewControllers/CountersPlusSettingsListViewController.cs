using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using HMUI;
using CountersPlus.Config;
using IniParser.Model;
using IniParser;
using CountersPlus.Custom;
using TMPro;
using IPA.Loader;
using IPA.Old;

namespace CountersPlus.UI.ViewControllers
{
    /*
     * Now with CountersPlusHorizontalViewController, this is no longer necessary.
     * 
     * Bottom screen looks better, plus you dont need to look to the left and right everytime to change settings.
     */ 
    class CountersPlusSettingsListViewController : CustomListViewController
    {
        private LevelListTableCell cellInstance;
        internal static CountersPlusSettingsListViewController Instance;
        public List<SettingsInfo> counterInfos = new List<SettingsInfo>();

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    Instance = this;
                    cellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First((LevelListTableCell x) => x.name == "LevelListTableCell");
                    base.DidActivate(firstActivation, type);

                    foreach (var kvp in AdvancedCounterSettings.counterUIItems) counterInfos.Add(CreateFromModel(kvp.Key));
                    FileIniDataParser parser = new FileIniDataParser();
                    IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
                    foreach (SectionData section in data.Sections)
                    {
                        if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                        {
                            CustomConfigModel potential = new CustomConfigModel(section.SectionName);
                            potential = ConfigLoader.DeserializeFromConfig(potential, section.SectionName) as CustomConfigModel;
                            if (PluginManager.GetPlugin(section.Keys["ModCreator"]) == null &&
                            #pragma warning disable CS0618 //Fuck off DaNike
                            PluginManager.Plugins.Where((IPlugin x) => x.Name == section.Keys["ModCreator"]).FirstOrDefault() == null) continue;
                            counterInfos.Add(new SettingsInfo()
                            {
                                Name = potential.DisplayName,
                                Description = $"A custom counter added by {potential.ModCreator}!",
                                Model = potential,
                                IsCustom = true,
                            });
                        }
                    }
                    _customListTableView.didSelectCellWithIdxEvent += OnCellSelect;
                    _customListTableView.ReloadData();
                    _customListTableView.SelectCellWithIdx(0, false);
                }
            }
            catch (Exception e) {
                Plugin.Log(e.Message + e.StackTrace, Plugin.LogInfo.Fatal, "Check dependencies. If issue persists, re-install Counters+. If issue still persists, create an issue on the Counters+ GitHub.");
            }
        }

        private SettingsInfo CreateFromModel<T>(T settings) where T : ConfigModel
        {
            SettingsInfo info = new SettingsInfo()
            {
                Name = settings.DisplayName,
                Description = DescriptionForModel(settings),
                Model = settings,
            };
            return info;
        }

        private string DescriptionForModel<T>(T settings) where T : ConfigModel
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

        public override float CellSize() { return 10f; }

        public override int NumberOfCells() { return counterInfos.Count + 3; }

        public override TableCell CellForIdx(int row)
        {
            LevelListTableCell cell = Instantiate(cellInstance);
            var beatmapCharacteristicImages = cell.GetPrivateField<UnityEngine.UI.Image[]>("_beatmapCharacteristicImages");
            foreach (UnityEngine.UI.Image i in beatmapCharacteristicImages) i.enabled = false;
            cell.SetPrivateField("_beatmapCharacteristicAlphas", new float[0]);
            cell.SetPrivateField("_beatmapCharacteristicImages", new UnityEngine.UI.Image[0]);
            TextMeshProUGUI songName = cell.GetPrivateField<TextMeshProUGUI>("_songNameText");
            TextMeshProUGUI author = cell.GetPrivateField<TextMeshProUGUI>("_authorText");
            author.overflowMode = TextOverflowModes.Overflow;
            RawImage image = cell.GetPrivateField<RawImage>("_coverRawImage");
            if (row == 0)
            {
                songName.text = "Main Settings";
                author.text = "Configure basic Counters+ settings.";
                image.texture = Images.Images.LoadTexture("MainSettings");
            }
            else if (row == NumberOfCells() - 2)
            {
                songName.text = "Credits";
                author.text = "View credits for Counters+.";
                image.texture = Images.Images.LoadTexture("Credits");
            }
            else if (row == NumberOfCells() - 1) {
                songName.text = "Contributors";
                author.text = "See who helped with Counters+!";
                image.texture = Images.Images.LoadTexture("Contributors");
            }
            else
            {
                SettingsInfo info = counterInfos[row - 1];
                songName.text = info.Name;
                author.text = info.Description;
                try
                {
                    if (info.IsCustom) image.texture = Images.Images.LoadTexture("Custom");
                    else image.texture = Images.Images.LoadTexture(info.Name);
                }
                catch { image.texture = Images.Images.LoadTexture("Bug"); }
            }
            cell.reuseIdentifier = "CountersPlusSettingCell";
            return cell;
        }

        private void OnCellSelect(TableView view, int row)
        {
            SettingsInfo info = null;
            if (row > 0 && row < NumberOfCells() - 2) info = counterInfos[row - 1];
            if (row == 0) CountersPlusEditViewController.ShowMainSettings();
            else if (row == NumberOfCells() - 2) CountersPlusEditViewController.CreateFiller();
            else if (row == NumberOfCells() - 1) CountersPlusEditViewController.ShowContributors();
            else CountersPlusEditViewController.UpdateSettings(info.Model, info);
        }
    }

    class SettingsInfo{
        public string Name;
        public string Description;
        public ConfigModel Model;
        public bool IsCustom = false;
    }
}
