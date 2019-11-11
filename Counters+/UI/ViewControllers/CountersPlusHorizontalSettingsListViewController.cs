using System;
using System.Linq;
using System.Collections.Generic;
using CountersPlus.Config;
using CountersPlus.Custom;
using CountersPlus.Utils;
using CustomUI.BeatSaber;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
     * 
     * I've left comments on whether or not you should keep or remove certain segments of code. Happy code yoinking!
     */
    class CountersPlusHorizontalSettingsListViewController : CustomViewController, TableView.IDataSource
    {
        internal static CountersPlusHorizontalSettingsListViewController Instance;
        public List<SettingsInfo> counterInfos = new List<SettingsInfo>();

        private Button PageLeftButton;
        private Button PageRightButton;
        private TableView CustomListTableView;
        private readonly string ReuseIdentifier = "CountersPlusSettingsListTableCell"; //Change this if you plan on yoinking my code.
        private LevelPackTableCell levelPackTableCellInstance;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    //Firstly, load my Counter settings and data, as its necessary for the NumberOfCells function.
                    //This segment of code can be removed.
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

                    //Largely unchanged from CustomListController. Keep all of this.
                    Instance = this;
                    levelPackTableCellInstance = Resources.FindObjectsOfTypeAll<LevelPackTableCell>().First(x => x.name == "LevelPackTableCell");
                    levelPackTableCellInstance.reuseIdentifier = ReuseIdentifier;

                    RectTransform container = new GameObject("HorizontalListContainer", typeof(RectTransform)).transform as RectTransform;
                    container.SetParent(rectTransform, false);
                    container.sizeDelta = Vector2.zero;
                    container.anchorMin = new Vector2(0.1f, 0); //Squish the list container a little bit
                    container.anchorMax = new Vector2(0.9f, 1); //To make room for the forward/backward buttons

                    var go = new GameObject("CustomListTableView");
                    go.SetActive(false); //Disable GameObject to not have scripts run.
                    CustomListTableView = go.AddComponent<TableView>(); //Add TableView
                    CustomListTableView.gameObject.AddComponent<RectMask2D>(); //Add Mask
                    CustomListTableView.transform.SetParent(container, false);
                    CustomListTableView.SetPrivateField("_tableType", TableView.TableType.Horizontal);

                    (CustomListTableView.transform as RectTransform).anchorMin = Vector2.zero;
                    (CustomListTableView.transform as RectTransform).anchorMax = Vector2.one;
                    (CustomListTableView.transform as RectTransform).sizeDelta = Vector2.zero;
                    (CustomListTableView.transform as RectTransform).anchoredPosition = Vector2.zero;

                    CustomListTableView.SetPrivateField("_preallocatedCells", new TableView.CellsGroup[0]);
                    CustomListTableView.SetPrivateField("_isInitialized", false);

                    //Code copied from monkeymanboy's Beat Saber Custom Campaigns mod. Keep these too.
                    PageLeftButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == "PageLeftButton")), transform);
                    RectTransform buttonTransform = PageLeftButton.transform.Find("BG") as RectTransform;
                    RectTransform glow = Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Last(x => (x.name == "GlowContainer")), PageLeftButton.transform).transform as RectTransform;
                    glow.localPosition = buttonTransform.localPosition;
                    glow.anchoredPosition = buttonTransform.anchoredPosition;
                    glow.anchorMin = buttonTransform.anchorMin;
                    glow.anchorMax = buttonTransform.anchorMax;
                    glow.sizeDelta = buttonTransform.sizeDelta;
                    PageLeftButton.transform.localPosition = new Vector3(-110, 2.5f, -5);
                    PageLeftButton.interactable = true;
                    PageRightButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == "PageRightButton")), transform);
                    buttonTransform = PageRightButton.transform.Find("BG") as RectTransform;
                    glow = Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Last(x => (x.name == "GlowContainer")), PageRightButton.transform).transform as RectTransform;
                    glow.localPosition = buttonTransform.localPosition;
                    glow.anchoredPosition = buttonTransform.anchoredPosition;
                    glow.anchorMin = buttonTransform.anchorMin;
                    glow.anchorMax = buttonTransform.anchorMax;
                    glow.sizeDelta = buttonTransform.sizeDelta;
                    PageRightButton.transform.localPosition = new Vector3(110, 2.5f, -5);
                    PageRightButton.interactable = true;

                    RectTransform viewport = new GameObject("Viewport").AddComponent<RectTransform>(); //Make a Viewport RectTransform
                    viewport.SetParent(CustomListTableView.transform as RectTransform, false); //It expects one from a ScrollRect, so we have to make one ourselves.
                    viewport.sizeDelta = Vector2.zero; //Important to set this to zero so the TableView can scroll through all available cells.

                    CustomListTableView.SetPrivateField("_pageUpButton", PageLeftButton); //Set Up button to Left
                    CustomListTableView.SetPrivateField("_pageDownButton", PageRightButton); //Set down button to Right
                    CustomListTableView.Init(); //Init, have "_scrollRectTransform" be null.
                    CustomListTableView.SetPrivateField("_scrollRectTransform", viewport); //Set it with our hot-out-of-the-oven Viewport.
                    CustomListTableView.dataSource = this; //Add data source
                    go.SetActive(true);

                    //Finally, reset some anchors of each child so that all of the available cells are shown.
                    for (int i = 0; i < CustomListTableView.transform.childCount; i++)
                    {
                        (CustomListTableView.transform.GetChild(i).transform as RectTransform).anchoredPosition = Vector3.zero;
                        (CustomListTableView.transform.GetChild(i).transform as RectTransform).anchorMin = Vector3.zero;
                        (CustomListTableView.transform.GetChild(i).transform as RectTransform).anchorMax = Vector3.one;
                    }

                    //Reload the data, and select the first cell in the list.
                    CustomListTableView.didSelectCellWithIdxEvent += OnCellSelect;
                    CustomListTableView.ReloadData();
                    CustomListTableView.SelectCellWithIdx(0, false);
                }
            } catch (Exception e)
            {  //Edit this with your logging system of choice, or delete it altogether (As this shouldn't really cause Exceptions)
                Plugin.Log(e.ToString(), LogInfo.Error, "Report this as an issue on the Counters+ GitHub.");
            }
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

        //I'd recommend keeping this as is (6 cells shown), unless you want more spread out cells.
        public float CellSize() { return 27.5f; }

        public int NumberOfCells() { return counterInfos.Count + 3; } //Tune this to the amount of cells you'll have, whether dynamic or static.

        public TableCell CellForIdx(TableView view, int row) //Here is where you customize your TableCell.
        {
            LevelPackTableCell cell = CustomListTableView.DequeueReusableCellForIdentifier(ReuseIdentifier) as LevelPackTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                cell = Instantiate(levelPackTableCellInstance);
                cell.reuseIdentifier = ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.
            TextMeshProUGUI packNameText = cell.GetPrivateField<TextMeshProUGUI>("_packNameText"); //Grab some TMP references.
            TextMeshProUGUI packInfoText = cell.GetPrivateField<TextMeshProUGUI>("_infoText");
            packInfoText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            UnityEngine.UI.Image packCoverImage = cell.GetPrivateField<UnityEngine.UI.Image>("_coverImage");
            packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images (especially transparent ones)
            if (row == 0) //From here on out is mainly Counters+ things, so you can safely remove them.
            {
                packNameText.text = "Main Settings";
                packInfoText.text = "Configure basic Counters+ settings.";
                packCoverImage.sprite = Images.Images.LoadSprite("MainSettings");
            }
            else if (row == NumberOfCells() - 1)
            {
                packNameText.text = "Donators";
                packInfoText.text = "See who supported me on Ko-fi!";
                packCoverImage.sprite = Images.Images.LoadSprite("Donators");
                cell.showNewRibbon = true;
            }
            else if (row == NumberOfCells() - 2)
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
            }
            return cell;
        }

        private void OnCellSelect(TableView view, int row) //Change what happens on cell select.
        {
            SettingsInfo info = null;
            if (row > 0 && row < NumberOfCells() - 2) info = counterInfos[row - 1];
            if (row == 0) CountersPlusEditViewController.ShowMainSettings();
            else if (row == NumberOfCells() - 1) CountersPlusEditViewController.ShowDonators();
            else if (row == NumberOfCells() - 2) CountersPlusEditViewController.ShowContributors();
            else CountersPlusEditViewController.UpdateSettings(info.Model);
        }
    }

    class SettingsInfo
    {
        public string Name;
        public string Description;
        public ConfigModel Model;
        public bool IsCustom = false;
    }
}
