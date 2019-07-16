using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CountersPlus.Config;
using CountersPlus.Counters;
using CountersPlus.Custom;
using UnityEngine.SceneManagement;
using IPA.Loader;
using IniParser;
using IniParser.Model;
using System.Collections;
using IPA.Old;

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
                Plugin.Log("Counters Controller created.", Plugin.LogInfo.Notice);
            }
        }

        private void Awake()
        {
            SceneManager.activeSceneChanged += ActiveSceneChanged;
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= ActiveSceneChanged;
        }

        private void ActiveSceneChanged(Scene arg, Scene arg1)
        {
            LoadedCounters.Clear();
        }

        static void LoadCounter<T, R>(string name, T settings) where T : ConfigModel
        {
            if (!settings.Enabled || GameObject.Find("Counters+ | " + name + " Counter")) return;
            GameObject counter = new GameObject("Counters+ | " + name + " Counter");
            counter.transform.position = DeterminePosition(counter, settings.Position, settings.Index);
            counter.AddComponent(typeof(R));
            Plugin.Log("Loaded Counter: " + name);
            LoadedCounters.Add(counter);
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
            catch { Plugin.Log($"Can't remove the {Name} counter!", Plugin.LogInfo.Warning); }
        }

        public static void LoadCounters()
        {
            Plugin.Log("Loading Counters...", Plugin.LogInfo.Notice);
            LoadCounter<MissedConfigModel, MissedCounter>("Missed", settings.missedConfig);
            LoadCounter<NoteConfigModel, AccuracyCounter>("Notes", settings.noteConfig);
            LoadCounter<ScoreConfigModel, ScoreCounter>("Score", settings.scoreConfig);
            LoadCounter<PBConfigModel, PBCounter>("Personal Best", settings.pbConfig);
            LoadCounter<ProgressConfigModel, ProgressCounter>("Progress", settings.progressConfig);
            LoadCounter<SpeedConfigModel, SpeedCounter>("Speed", settings.speedConfig);
            LoadCounter<CutConfigModel, CutCounter>("Cut", settings.cutConfig);
            LoadCounter<SpinometerConfigModel, Spinometer>("Spinometer", settings.spinometerConfig);
            LoadCounter<NotesLeftConfigModel, NotesLeftCounter>("Notes Left", settings.notesLeftConfig);
            LoadCounter<FailConfigModel, FailCounter>("Fail", settings.failsConfig);
            Plugin.Log("Counters loaded!", Plugin.LogInfo.Notice);
            Instance.StartCoroutine(Instance.ObtainRequiredData());

            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            foreach (SectionData section in data.Sections)
            {
                if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                {
                    if (PluginManager.GetPlugin(section.Keys["ModCreator"]) == null &&
                        #pragma warning disable CS0618 //Fuck off DaNike
                        PluginManager.Plugins.Where((IPlugin x) => x.Name == section.Keys["ModCreator"]).FirstOrDefault() == null) return;
                    CustomConfigModel potential = new CustomConfigModel(section.SectionName);
                    potential = ConfigLoader.DeserializeFromConfig(potential, section.SectionName) as CustomConfigModel;
                    LoadCounter<CustomConfigModel, CustomCounterHook>(section.Keys["SectionName"], potential);
                }
            }
        }

        public static Vector3 DeterminePosition(GameObject counter, ICounterPositions position, int index)
        {
            float X = 3.2f;
            Vector3 pos = new Vector3(); //Base position
            Vector3 offset = new Vector3(0, -0.75f * (index), 0); //Offset for any overlapping, indexes, etc.
            bool nextToProgress = settings.progressConfig.Position == position && settings.progressConfig.Index < index && settings.progressConfig.Mode == ICounterMode.Original;
            bool nextToScore = settings.scoreConfig.Position == position && settings.scoreConfig.Index < index;
            bool baseScore = settings.scoreConfig.Mode == ICounterMode.BaseGame;
            switch (position)
            {
                case ICounterPositions.BelowCombo:
                    pos = new Vector3(-X, 0.85f - settings.ComboOffset, 7);
                    if (nextToProgress) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore && baseScore) offset += new Vector3(0, -0.15f, 0);
                    break;
                case ICounterPositions.AboveCombo:
                    pos = new Vector3(-X, 1.7f + settings.ComboOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    if (nextToProgress) offset -= new Vector3(0, -0.5f, 0);
                    break;
                case ICounterPositions.BelowMultiplier:
                    pos = new Vector3(X, 0.75f - settings.MultiplierOffset, 7);
                    if (GameObject.Find("FCDisplay")) offset += new Vector3(0, -0.25f, 0);
                    if (nextToProgress) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore && baseScore) offset += new Vector3(0, -0.15f, 0);
                    break;
                case ICounterPositions.AboveMultiplier:
                    pos = new Vector3(X, 1.7f + settings.MultiplierOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    if (GameObject.Find("FCDisplay")) offset += new Vector3(0, -0.25f, 0);
                    if (nextToProgress) offset -= new Vector3(0, -0.5f, 0);
                    break;
                case ICounterPositions.BelowEnergy:
                    pos = new Vector3(0, -1.5f, 7);
                    if (nextToProgress) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore && baseScore) offset += new Vector3(0, -0.15f, 0);
                    break;
                case ICounterPositions.AboveHighway:
                    pos = new Vector3(0, 2.5f, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    if (nextToProgress) offset -= new Vector3(0, -0.5f, 0);
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
