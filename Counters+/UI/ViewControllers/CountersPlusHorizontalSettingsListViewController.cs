using System;
using System.Collections.Generic;
using System.Linq;
using CountersPlus.Config;
using CountersPlus.Custom;
using CustomUI.BeatSaber;
using HMUI;
using IPA.Loader;
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
        private string ReuseIdentifier = "CountersPlusSettingsListTableCell"; //Change this if you plan on yoinking my code.
        private LevelPackTableCell levelPackTableCellInstance;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    //Largely unchanged from CustomListController. Keep all of this.
                    Instance = this;
                    levelPackTableCellInstance = Resources.FindObjectsOfTypeAll<LevelPackTableCell>().First(x => x.name == "LevelPackTableCell");
                    levelPackTableCellInstance.reuseIdentifier = ReuseIdentifier;

                    RectTransform container = new GameObject("HorizontalListContainer", typeof(RectTransform)).transform as RectTransform;
                    container.SetParent(rectTransform, false);
                    container.sizeDelta = new Vector2(0, 0);
                    container.anchorMin = new Vector2(0.1f, 0); //Squish the list container a little bit
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

                    //Code copied from monkeymanboy's Beat Saber Custom Campaigns mod. Keep these too.
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
                    PageLeftButton.onClick.AddListener(() => CustomListTableView.PageScrollUp());
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
                    PageRightButton.onClick.AddListener(() => CustomListTableView.PageScrollDown());

                    //Now load my Counter settings and data. The rest from here on out is mainly copied from
                    //the old CountersPlusSettingsViewController, and can be safely removed.
                    foreach (var kvp in AdvancedCounterSettings.counterUIItems) counterInfos.Add(CreateFromModel(kvp.Key));
                    foreach(CustomConfigModel potential in ConfigLoader.LoadCustomCounters())
                    {
                        counterInfos.Add(new SettingsInfo()
                        {
                            Name = potential.DisplayName,
                            Description = $"A custom counter added by {potential.ModCreator}!",
                            Model = potential,
                            IsCustom = true,
                        });
                    }

                    //Reload the data, and select the first cell in the list.
                    CustomListTableView.didSelectCellWithIdxEvent += OnCellSelect;
                    CustomListTableView.ReloadData();
                    CustomListTableView.SelectCellWithIdx(0, false);
                }
            } catch (Exception e)
            {  //Edit this with your logging system of choice, or delete it altogether (As this shouldn't really cause Exceptions)
                Plugin.Log(e.ToString(), Plugin.LogInfo.Error, "Report this as an issue on the Counters+ GitHub.");
            }
        }

        private SettingsInfo CreateFromModel<T>(T settings) where T : ConfigModel //Counters+ stuff, OK to remove.
        {
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

        //I'd recommend keeping this as is (5 cells shown), unless you want more spread out cells (40 = 4 cells shown).
        public float CellSize() { return 30f; }

        public int NumberOfCells() { return counterInfos.Count + 3; } //Tune this to the amount of cells you'll have, whether dynamic or static.

        public TableCell CellForIdx(int row) //Here is where you customize your TableCell.
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
                        packCoverImage.sprite = Images.Images.LoadSprite("Custom");
                        cell.showNewRibbon = (info.Model as CustomConfigModel).IsNew;
                    }
                    else
                    {
                        packCoverImage.sprite = Images.Images.LoadSprite(info.Name);
                        if (info.Model.VersionAdded != null)
                        {
                            cell.showNewRibbon = PluginManager.GetPlugin("Counters+").Metadata.Version == info.Model.VersionAdded;
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
            else CountersPlusEditViewController.UpdateSettings(info.Model, info);
        }
    }
}
