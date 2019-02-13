using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using VRUI;
using HMUI;
using CountersPlus.Config;
using IniParser.Model;
using IniParser;
using CountersPlus.Custom;
using IllusionInjector;
using IllusionPlugin;
using System.Collections;
using CustomUI.Utilities;

namespace CountersPlus.UI
{
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

                    foreach (var kvp in AdvancedCounterSettings.counterUIItems)
                    {
                        //counterInfos.Add(kvp.Key, CreateFromModel(kvp.Key));
                        counterInfos.Add(CreateFromModel(kvp.Key));
                    }
                    FileIniDataParser parser = new FileIniDataParser();
                    IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
                    foreach (SectionData section in data.Sections)
                    {
                        if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                        {
                            CustomConfigModel potential = new CustomConfigModel(section.SectionName);
                            if (!PluginManager.Plugins.Any((IPlugin x) => x.Name == section.Keys["ModCreator"])) continue;
                            counterInfos.Add(new SettingsInfo()
                            {
                                Name = potential.DisplayName,
                                Description = $"A custom counter added by {potential.ModCreator}!",
                                Model = potential,
                                IsCustom = true,
                            });
                        }
                    }
                    _customListTableView.didSelectRowEvent += onCellSelect;
                    _customListTableView.ReloadData();
                }
            }
            catch (Exception e) {
                Plugin.Log(e.ToString(), Plugin.LogInfo.Fatal);
            }
        }

        private SettingsInfo CreateFromModel<T>(T settings) where T : IConfigModel
        {
            SettingsInfo info = new SettingsInfo()
            {
                Name = settings.DisplayName,
                Description = DescriptionForModel(settings),
                Model = settings,
            };
            return info;
        }

        private string DescriptionForModel<T>(T settings) where T : IConfigModel
        {
            switch (settings.DisplayName)
            {
                default:
                    return "Huh, I dont know, I cant find this!";
                case "Missed":
                    return "<i>MISS</i>";
                case "Notes":
                    return "Notes hit over total notes!";
                case "Progress":
                    return "The original you know and love.";
                case "Score":
                    return "If the in-game score counter wasn't good enough.";
                case "Speed":
                    return "<i>\"Speed, motherfucker, do you speak it?\"</i>";
                case "Cut":
                    return "How well you hit those bloqs.";
                case "Spinometer":
                    return "Can you break 3600 degrees per second?";
            }
        }

        public override float RowHeight()
        {
            return 10f;
        }

        public override int NumberOfRows()
        {
            return counterInfos.Count + 2;
        }

        public override TableCell CellForRow(int row)
        {
            LevelListTableCell cell = Instantiate(cellInstance);
            if (row == 0)
            {
                cell.songName = "Main Settings";
                cell.author = "Configure basic Counters+ settings.";
            }
            else if (row == NumberOfRows() - 1)
            {
                cell.songName = "Credits";
                cell.author = "View credits for Counters+.";
            }else
            {
                SettingsInfo info = counterInfos[row - 1];
                cell.songName = info.Name;
                cell.author = info.Description;
            }
            cell.coverImage = Sprite.Create(Texture2D.blackTexture, new Rect(), Vector2.zero);
            cell.reuseIdentifier = "CountersPlusSettingCell";
            return cell;
        }

        private void onCellSelect(TableView view, int row)
        {
            if (row != 0 && row != NumberOfRows() - 1)
            {
                SettingsInfo info = counterInfos[row - 1];
                StartCoroutine(WaitForSettings(info));
            }
            else
            {
                CountersPlusEditViewController.UpdateSettings(null as IConfigModel, counterInfos.First(), true, (row == NumberOfRows() - 1));
            }
        }

        IEnumerator WaitForSettings(SettingsInfo info)
        {
            yield return new WaitUntil(() => !String.IsNullOrWhiteSpace(info.Model.DisplayName));
            yield return new WaitUntil(() => info.Name != null);
            CountersPlusEditViewController.UpdateSettings(info.Model, info);
            Plugin.Log("Loading settings for: " + info.Name);
        }
    }

    class SettingsInfo{
        public string Name;
        public string Description;
        public IConfigModel Model;
        public bool IsCustom = false;
    }
}
