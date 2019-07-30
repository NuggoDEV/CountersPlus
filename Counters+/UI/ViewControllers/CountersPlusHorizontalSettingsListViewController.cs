using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountersPlus.Config;
using CountersPlus.Custom;
using CustomUI.BeatSaber;
using HMUI;
using IniParser;
using IniParser.Model;
using IPA.Loader;
using IPA.Old;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CountersPlus.UI.ViewControllers
{
    /*
     * I've copied over a large amount of this code from CustomUI's CustomListController,
     * mainly because it would need more editing then what simple overrides would allow to
     * achieve the horizontal effect that I want, and I didn't want to go through Harmony patches to do it.
     * 
     * Might as well make it from scratch, with CustomListController as a starting point.
     *
     * In the end, though, I'm really happy with how this turned out.
     */
    class CountersPlusHorizontalSettingsListViewController : CustomViewController, TableView.IDataSource
    {
        internal static CountersPlusHorizontalSettingsListViewController Instance;
        public List<SettingsInfo> counterInfos = new List<SettingsInfo>();

        private Button PageLeftButton;
        private Button PageRightButton;
        private TableView CustomListTableView;
        private string ReuseIdentifier = "CountersPlusSettingsListTableCell"; //Change this if you plan on yoinking my code.
        private LevelPackTableCell levelPackTableCellInstance;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    //Largely unchanged from CustomListController.
                    Instance = this;
                    levelPackTableCellInstance = Resources.FindObjectsOfTypeAll<LevelPackTableCell>().First(x => x.name == "LevelPackTableCell");
                    levelPackTableCellInstance.reuseIdentifier = ReuseIdentifier;

                    RectTransform container = new GameObject("HorizontalListContainer", typeof(RectTransform)).transform as RectTransform;
                    container.SetParent(rectTransform, false);
                    container.sizeDelta = new Vector2(0, 0);
                    container.anchorMin = new Vector2(0.1f, 0); //Squish the list container a little bit inside
                    container.anchorMax = new Vector2(0.9f, 1); //To make room for the forward/backward buttons

                    var go = new GameObject("CustomListTableView");
                    go.SetActive(false);
                    CustomListTableView = go.AddComponent<TableView>();
                    CustomListTableView.gameObject.AddComponent<RectMask2D>();
                    CustomListTableView.transform.SetParent(container, false);
                    CustomListTableView.SetPrivateField("_tableType", TableView.TableType.Horizontal);

                    (CustomListTableView.transform as RectTransform).anchorMin = new Vector2(0, 0);
                    (CustomListTableView.transform as RectTransform).anchorMax = new Vector2(1, 1);
                    (CustomListTableView.transform as RectTransform).sizeDelta = new Vector2(0, 0);
                    (CustomListTableView.transform as RectTransform).anchoredPosition = new Vector2(0, 0);

                    CustomListTableView.SetPrivateField("_preallocatedCells", new TableView.CellsGroup[0]);
                    CustomListTableView.SetPrivateField("_isInitialized", false);
                    CustomListTableView.dataSource = this;
                    go.SetActive(true);

                    //Code copied from monkeymanboy's Beat Saber Custom Campaigns mod.
                    PageLeftButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == "PageLeftButton")), transform);
                    RectTransform buttonTransform = PageLeftButton.transform.Find("BG") as RectTransform;
                    RectTransform glow = Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Last(x => (x.name == "GlowContainer")), PageLeftButton.transform).transform as RectTransform;
                    glow.localPosition = buttonTransform.localPosition;
                    glow.anchoredPosition = buttonTransform.anchoredPosition;
                    glow.anchorMin = buttonTransform.anchorMin;
                    glow.anchorMax = buttonTransform.anchorMax;
                    glow.sizeDelta = buttonTransform.sizeDelta;
                    PageLeftButton.transform.localPosition = new Vector3(-80, 2.5f, -5);
                    PageLeftButton.interactable = true;
                    PageLeftButton.onClick.AddListener(delegate ()
                    {
                        CustomListTableView.PageScrollUp();
                    });
                    PageRightButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == "PageRightButton")), transform);
                    buttonTransform = PageRightButton.transform.Find("BG") as RectTransform;
                    glow = Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Last(x => (x.name == "GlowContainer")), PageRightButton.transform).transform as RectTransform;
                    glow.localPosition = buttonTransform.localPosition;
                    glow.anchoredPosition = buttonTransform.anchoredPosition;
                    glow.anchorMin = buttonTransform.anchorMin;
                    glow.anchorMax = buttonTransform.anchorMax;
                    glow.sizeDelta = buttonTransform.sizeDelta;
                    PageRightButton.transform.localPosition = new Vector3(80, 2.5f, -5);
                    PageRightButton.interactable = true;
                    PageRightButton.onClick.AddListener(delegate ()
                    {
                        CustomListTableView.PageScrollDown();
                    });

                    //Now load my Counter settings and data. The rest from here on out is mainly copied from
                    //the old CountersPlusSettingsViewController.
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

                    CustomListTableView.didSelectCellWithIdxEvent += OnCellSelect;
                    CustomListTableView.ReloadData();
                    CustomListTableView.SelectCellWithIdx(0, false);
                }
            } catch (Exception e)
            {
                Plugin.Log(e.ToString(), Plugin.LogInfo.Error, "Report this as an issue on the Counters+ GitHub.");
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

        public float CellSize() { return 30f; }

        public int NumberOfCells() { return counterInfos.Count + 3; }

        public TableCell CellForIdx(int row)
        {
            LevelPackTableCell cell = CustomListTableView.DequeueReusableCellForIdentifier(ReuseIdentifier) as LevelPackTableCell;
            cell.showNewRibbon = false;
            if (cell == null)
            {
                cell = Instantiate(levelPackTableCellInstance);
                cell.reuseIdentifier = ReuseIdentifier;
            }
            TextMeshProUGUI packNameText = cell.GetPrivateField<TextMeshProUGUI>("_packNameText");
            TextMeshProUGUI packInfoText = cell.GetPrivateField<TextMeshProUGUI>("_infoText");
            packInfoText.richText = true; //Enable rich text for info text.
            UnityEngine.UI.Image packCoverImage = cell.GetPrivateField<UnityEngine.UI.Image>("_coverImage");
            if (row == 0)
            {
                packNameText.text = "Main Settings";
                packInfoText.text = "Configure basic Counters+ settings.";
                packCoverImage.sprite = Images.Images.LoadSprite("MainSettings");
            }
            else if (row == NumberOfCells() - 2)
            {
                packNameText.text = "Credits";
                packInfoText.text = "View credits for Counters+.";
                packCoverImage.sprite = Images.Images.LoadSprite("Credits");
            }
            else if (row == NumberOfCells() - 1)
            {
                packNameText.text = "Contributors";
                packInfoText.text = "See who helped with Counters+!";
                packCoverImage.sprite = Images.Images.LoadSprite("Contributors");
            }
            else
            {
                SettingsInfo info = counterInfos[row - 1];
                packNameText.text = info.Name;
                packInfoText.text = info.Description;
                cell.showNewRibbon = true;
                try
                {
                    if (info.IsCustom) packCoverImage.sprite = Images.Images.LoadSprite("Custom");
                    else packCoverImage.sprite = Images.Images.LoadSprite(info.Name);
                }
                catch { packCoverImage.sprite = Images.Images.LoadSprite("Bug"); }
            }
            return cell;
        }

        private void OnCellSelect(TableView view, int row)
        {
            SettingsInfo info = null;
            if (row > 0 && row < NumberOfCells() - 2) info = counterInfos[row - 1];
            if (row == 0) CountersPlusEditViewController.ShowMainSettings();
            else if (row == NumberOfCells() - 2) CountersPlusEditViewController.CreateCredits();
            else if (row == NumberOfCells() - 1) CountersPlusEditViewController.ShowContributors();
            else CountersPlusEditViewController.UpdateSettings(info.Model, info);
        }
    }
}
