using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CountersPlus.Config;
using CountersPlus.Counters;
using CountersPlus.Custom;
using UnityEngine.SceneManagement;
using IllusionInjector;
using IllusionPlugin;
using IniParser;
using IniParser.Model;
using TMPro;
using System.Collections;

namespace CountersPlus
{
    public class CountersController : MonoBehaviour
    {
        public static CountersController Instance { get; private set; }
        public static List<GameObject> loadedCounters { get; private set; } = new List<GameObject>();
        internal static MainConfigModel settings;
        internal static List<CustomCounter> customCounters { get; private set; } = new List<CustomCounter>();

        public static void OnLoad()
        {
            settings = ConfigLoader.LoadSettings();
            if (Instance == null && settings.Enabled)
            {
                GameObject controller = new GameObject("Counters+ | Controller");
                DontDestroyOnLoad(controller);
                Instance = controller.AddComponent<CountersController>();
                Plugin.Log("Controller | Counters Controller created.");
            }
        }

        void Awake()
        {
            SceneManager.activeSceneChanged += activeSceneChanged;
        }

        void activeSceneChanged(Scene arg, Scene arg1)
        {
            if (arg1.name == "Menu") loadedCounters.Clear();
        }

        static void LoadCounter<T, R>(string name, T settings) where T : IConfigModel
        {
            if (!settings.Enabled || GameObject.Find("Counters+ | " + name + " Counter")) return;
            GameObject counter = new GameObject("Counters+ | " + name + " Counter");
            counter.transform.position = determinePosition(counter, settings.Position, settings.Index);
            counter.AddComponent(typeof(R));
            Plugin.Log("Controller | Loaded Counter: " + name);
            loadedCounters.Add(counter);
        }

        public static void DelayedLoadCounters(float delay)
        {
            Instance.StartCoroutine(Instance.DelayLoad(delay));
        }

        private IEnumerator DelayLoad(float d)
        {
            yield return new WaitForSeconds(d);
            LoadCounters();
        }

        public static void LoadCounters()
        {
            Plugin.Log("Controller | Loading Counters...");
            LoadCounter<MissedConfigModel, MissedCounter>("Missed", settings.missedConfig);
            LoadCounter<NoteConfigModel, AccuracyCounter>("Notes", settings.noteConfig);
            LoadCounter<ScoreConfigModel, ScoreCounter>("Score", settings.scoreConfig);
            LoadCounter<ProgressConfigModel, ProgressCounter>("Progress", settings.progressConfig);
            LoadCounter<SpeedConfigModel, SpeedCounter>("Speed", settings.speedConfig);
            LoadCounter<CutConfigModel, CutCounter>("Cut", settings.cutConfig);
            LoadCounter<SpinometerConfigModel, Spinometer>("Spinometer", settings.spinometerConfig);
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            foreach (SectionData section in data.Sections)
            {
                if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                {
                    if (!PluginManager.Plugins.Any((IPlugin x) => x.Name == section.Keys["ModCreator"])) return;
                    CustomConfigModel potential = new CustomConfigModel(section.SectionName);
                    LoadCounter<CustomConfigModel, CustomCounterHook>(section.Keys["SectionName"], potential);
                }
            }
            if (settings.HideCombo)
            {
                for (int i = 0; i < GameObject.Find("ComboPanel").transform.childCount; i++)
                {
                    GameObject child = GameObject.Find("ComboPanel").transform.GetChild(i).gameObject;
                    if (child.name != "BG") Destroy(child);
                }
            }
            if (settings.HideMultiplier)
            {
                for (int i = 0; i < GameObject.Find("MultiplierPanel").transform.childCount; i++)
                {
                    GameObject child = GameObject.Find("MultiplierPanel").transform.GetChild(i).gameObject;
                    if (child.name != "BG") child.SetActive(false);
                }
            }
        }

        public static Vector3 determinePosition(GameObject counter, ICounterPositions position, int index)
        {
            float X = 3.2f;
            Vector3 pos = new Vector3(); //Base position
            Vector3 offset = new Vector3(0, -0.75f * (index), 0); //Offset for any overlapping, indexes, etc.
            bool nextToProgress = settings.progressConfig.Position == position && settings.progressConfig.Index < index && settings.progressConfig.Mode == ICounterMode.Original;
            bool nextToScore = settings.scoreConfig.Position == position && settings.scoreConfig.Index < index;
            bool baseScore = settings.scoreConfig.Mode == ICounterMode.BaseGame;
            switch (position)
            {
                case Config.ICounterPositions.BelowCombo:
                    pos = new Vector3(-X, 0.85f - settings.ComboOffset, 7);
                    if (nextToProgress) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore && baseScore) offset += new Vector3(0, -0.15f, 0);
                    break;
                case Config.ICounterPositions.AboveCombo:
                    pos = new Vector3(-X, 1.7f + settings.ComboOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    if (nextToProgress) offset -= new Vector3(0, -0.5f, 0);
                    break;
                case Config.ICounterPositions.BelowMultiplier:
                    pos = new Vector3(X, 0.75f - settings.MultiplierOffset, 7);
                    if (GameObject.Find("FCDisplay")) offset += new Vector3(0, -0.25f, 0);
                    if (nextToProgress) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore && baseScore) offset += new Vector3(0, -0.15f, 0);
                    break;
                case Config.ICounterPositions.AboveMultiplier:
                    pos = new Vector3(X, 1.7f + settings.MultiplierOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    if (GameObject.Find("FCDisplay")) offset += new Vector3(0, -0.25f, 0);
                    if (nextToProgress) offset -= new Vector3(0, -0.5f, 0);
                    break;
                case Config.ICounterPositions.BelowEnergy:
                    pos = new Vector3(0, -1.5f, 7);
                    if (nextToProgress) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore) offset += new Vector3(0, -0.25f, 0);
                    if (nextToScore && baseScore) offset += new Vector3(0, -0.15f, 0);
                    break;
                case Config.ICounterPositions.AboveHighway:
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
}
