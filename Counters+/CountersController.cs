using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CountersPlus.Utils;
using CountersPlus.Config;
using CountersPlus.Counters;
using CountersPlus.Custom;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CountersPlus
{
    public class CountersController : MonoBehaviour
    {
        public static CountersController Instance { get; private set; }
        public static List<GameObject> LoadedCounters { get; private set; } = new List<GameObject>();
        internal static MainConfigModel settings;

        public static Action<CountersData> ReadyToInit;

        public static void OnLoad()
        {
            settings = ConfigLoader.LoadSettings();
            if (Instance == null)
            {
                GameObject controller = new GameObject("Counters+ | Controller");
                DontDestroyOnLoad(controller);
                Instance = controller.AddComponent<CountersController>();
                controller.AddComponent<VersionChecker>();
                Plugin.Log("Counters Controller created.", LogInfo.Notice);
            }
        }

        /// <summary>
        /// Loads a counter given a ConfigModel and a MonoBehaviour. If enabled, a new GameObject will be created and the provided MonoBehaviour will be attached.
        /// </summary>
        /// <typeparam name="T">ConfigModel to use for settings.</typeparam>
        /// <typeparam name="R">MonoBehaviour to attach to the new GameObject.</typeparam>
        /// <param name="settings">ConfigModel settings reference.</param>
        internal static void LoadCounter<T, R>(T settings) where T : ConfigModel where R : Counter<T>
        {
            if (!settings.Enabled || GameObject.Find($"Counters+ | {settings.DisplayName} Counter")) return;
            R counter = new GameObject($"Counters+ | {settings.DisplayName} Counter").AddComponent(typeof(R)) as R;
            counter.settings = settings;
            Plugin.Log($"Loaded Counter: {settings.DisplayName}");
            LoadedCounters.Add(counter.gameObject);
        }

        private IEnumerator ObtainRequiredData()
        {
            Plugin.Log("Obtaining required counter data...");
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<ScoreController>().Any());
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<PlayerController>().Any());
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().Any());
            CountersData data = new CountersData();
            ReadyToInit.Invoke(data);
            Plugin.Log("Obtained data!");
            if (settings.HideCombo) HideUIElement("Combo");
            if (settings.HideMultiplier) HideUIElement("Multiplier");
        }

        private void HideUIElement(string Name)
        {
            try
            {
                for (int i = 0; i < GameObject.Find($"{Name}Panel").transform.childCount; i++)
                {
                    GameObject child = GameObject.Find($"{Name}Panel").transform.GetChild(i).gameObject;
                    if (child.name != "BG") child.SetActive(false);
                }
            }
            catch { Plugin.Log($"Can't remove the {Name} counter!", LogInfo.Warning); }
        }

        public static void LoadCounters()
        {
            Plugin.Log("Loading Counters...", LogInfo.Notice);
            LoadCounter<MissedConfigModel, MissedCounter>(settings.missedConfig);
            LoadCounter<NoteConfigModel, AccuracyCounter>(settings.noteConfig);
            LoadCounter<ScoreConfigModel, ScoreCounter>(settings.scoreConfig);
            LoadCounter<PBConfigModel, PBCounter>(settings.pbConfig);
            LoadCounter<ProgressConfigModel, ProgressCounter>(settings.progressConfig);
            LoadCounter<SpeedConfigModel, SpeedCounter>(settings.speedConfig);
            LoadCounter<CutConfigModel, CutCounter>(settings.cutConfig);
            LoadCounter<SpinometerConfigModel, Spinometer>(settings.spinometerConfig);
            LoadCounter<NotesLeftConfigModel, NotesLeftCounter>(settings.notesLeftConfig);
            LoadCounter<FailConfigModel, FailCounter>(settings.failsConfig);
            foreach (CustomCounter potential in CustomCounterCreator.LoadedCustomCounters)
                LoadCounter<CustomConfigModel, CustomCounterHook>(potential.ConfigModel);
            Plugin.Log("Counters loaded!", LogInfo.Notice);
            Instance.StartCoroutine(Instance.ObtainRequiredData());
        }

        public static Vector3 DeterminePosition(GameObject counter, ICounterPositions position, int index)
        {
            float X = 3.2f;
            Vector3 pos = new Vector3(); //Base position
            Vector3 offset = new Vector3(0, -0.75f * (index), 0); //Offset for any overlapping, indexes, etc.
            switch (position)
            {
                case ICounterPositions.BelowCombo:
                    pos = new Vector3(-X, 1.05f - settings.ComboOffset, 7);
                    break;
                case ICounterPositions.AboveCombo:
                    pos = new Vector3(-X, 1.9f + settings.ComboOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case ICounterPositions.BelowMultiplier:
                    pos = new Vector3(X, 0.95f - settings.MultiplierOffset, 7);
                    break;
                case ICounterPositions.AboveMultiplier:
                    pos = new Vector3(X, 1.9f + settings.MultiplierOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case ICounterPositions.BelowEnergy:
                    pos = new Vector3(0, -1.5f, 7);
                    break;
                case ICounterPositions.AboveHighway:
                    pos = new Vector3(0, 2.5f, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
            }
            if (counter.GetComponent<ProgressCounter>() != null)
                if (settings.progressConfig.Mode == ICounterMode.Original) offset += new Vector3(0.25f, 0, 0);
            return pos + offset;
        }
    }

    public class CountersData
    {
        public ScoreController ScoreController;
        public PlayerController PlayerController;
        public AudioTimeSyncController AudioTimeSyncController;
        public PlayerDataModelSO PlayerData;
        public GameplayModifiersModelSO ModifiersData;
        public GameplayCoreSceneSetupData GCSSD;

        public CountersData()
        {
            ScoreController = Resources.FindObjectsOfTypeAll<ScoreController>().First();
            PlayerController = Resources.FindObjectsOfTypeAll<PlayerController>().First();
            AudioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().First();
            PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().First();
            ModifiersData = Resources.FindObjectsOfTypeAll<GameplayModifiersModelSO>().First();
            GCSSD = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData; //By the time all of these load, so should GCSSD.
        }
    }
}
