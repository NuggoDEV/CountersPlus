using System;
using System.Collections.Generic;
using System.Linq;
using CountersPlus.UI.ViewControllers.SettingsGroups;
using CountersPlus.Utils;
using HMUI;
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
    public class CountersPlusHorizontalSettingsListViewController : ViewController, TableView.IDataSource
    {

        internal Button PageLeftButton;
        internal Button PageRightButton;
        private TableView CustomListTableView;
        internal readonly string ReuseIdentifier = "CountersPlusSettingsListTableCell"; //Change this if you plan on yoinking my code.
        internal LevelPackTableCell levelPackTableCellInstance;

        private List<SettingsGroup> loadedSettingsGroups = new List<SettingsGroup>();
        private SettingsGroup selectedSettingsGroup;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    loadedSettingsGroups = TypesUtility.GetListOfType<SettingsGroup>();
                    //Largely unchanged from CustomListController. Keep all of this.
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
                    PageLeftButton.transform.localPosition = new Vector3(-80, 2.5f, -5);
                    (PageLeftButton.transform as RectTransform).anchoredPosition = Vector3.up * 2.5f;
                    PageLeftButton.interactable = true;
                    PageLeftButton.gameObject.SetActive(false);
                    PageRightButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == "PageRightButton")), transform);
                    buttonTransform = PageRightButton.transform.Find("BG") as RectTransform;
                    glow = Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Last(x => (x.name == "GlowContainer")), PageRightButton.transform).transform as RectTransform;
                    glow.localPosition = buttonTransform.localPosition;
                    glow.anchoredPosition = buttonTransform.anchoredPosition;
                    glow.anchorMin = buttonTransform.anchorMin;
                    glow.anchorMax = buttonTransform.anchorMax;
                    glow.sizeDelta = buttonTransform.sizeDelta;
                    PageRightButton.transform.localPosition = new Vector3(80, 2.5f, -5);
                    (PageRightButton.transform as RectTransform).anchoredPosition = Vector3.up * 2.5f;
                    PageRightButton.interactable = true;
                    PageRightButton.gameObject.SetActive(false);

                    RectTransform viewport = new GameObject("Viewport").AddComponent<RectTransform>(); //Make a Viewport RectTransform
                    viewport.SetParent(CustomListTableView.transform as RectTransform, false); //It expects one from a ScrollRect, so we have to make one ourselves.
                    viewport.sizeDelta = Vector2.zero; //Important to set this to zero so the TableView can scroll through all available cells.

                    CustomListTableView.SetPrivateField("_pageUpButton", PageLeftButton); //Set Up button to Left
                    CustomListTableView.SetPrivateField("_pageDownButton", PageRightButton); //Set down button to Right
                    CustomListTableView.InvokePrivateMethod("Init", null); //Init, have "_scrollRectTransform" be null.
                    CustomListTableView.SetPrivateField("_scrollRectTransform", viewport); //Set it with our hot-out-of-the-oven Viewport.
                    CustomListTableView.dataSource = this; //Add data source
                    go.SetActive(true);
                    CustomListTableView.Show();

                    //Finally, reset some anchors of each child so that all of the available cells are shown.
                    for (int i = 0; i < CustomListTableView.transform.childCount; i++)
                    {
                        (CustomListTableView.transform.GetChild(i).transform as RectTransform).anchoredPosition = Vector3.zero;
                        (CustomListTableView.transform.GetChild(i).transform as RectTransform).anchorMin = Vector3.zero;
                        (CustomListTableView.transform.GetChild(i).transform as RectTransform).anchorMax = Vector3.one;
                    }
                }
            } catch (Exception e)
            {  //Edit this with your logging system of choice, or delete it altogether (As this shouldn't really cause Exceptions)
                Plugin.Log(e.ToString(), LogInfo.Error, "Report this as an issue on the Counters+ GitHub.");
            }
            CustomListTableView.didSelectCellWithIdxEvent += OnCellSelect;
            HandleSettingsGroupChanged(SettingsGroupType.Main);
            CountersPlusBottomSettingsSelectorViewController.SettingsGroupChanged += HandleSettingsGroupChanged;
            (transform as RectTransform).anchoredPosition = Vector3.zero;
        }

        private void HandleSettingsGroupChanged(SettingsGroupType obj)
        {
            selectedSettingsGroup = loadedSettingsGroups.Where(x => x.type == obj).FirstOrDefault();
            CustomListTableView.ReloadData();
            CustomListTableView.SelectCellWithIdx(0);
            TableViewScroller scroller = CustomListTableView.GetPrivateField<TableViewScroller>("_scroller");
            scroller.ScrollToCellWithIdx(0, TableViewScroller.ScrollPositionType.Beginning, true);
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            CountersPlusBottomSettingsSelectorViewController.SettingsGroupChanged -= HandleSettingsGroupChanged;
            CustomListTableView.didSelectCellWithIdxEvent -= OnCellSelect;
            base.DidDeactivate(deactivationType);
        }

        //I'd recommend keeping this as is (5 cells shown), unless you want more spread out cells.
        public float CellSize() => selectedSettingsGroup?.CellSize() ?? 30f;

        public int NumberOfCells() => selectedSettingsGroup?.NumberOfCells() ?? 0; //Tune this to the amount of cells you'll have, whether dynamic or static.

        public TableCell CellForIdx(TableView view, int row) => selectedSettingsGroup?.CellForIdx(view, row, this) ?? null;

        private void OnCellSelect(TableView view, int row) => selectedSettingsGroup?.OnCellSelect(view, row, this);
    }

    
}
