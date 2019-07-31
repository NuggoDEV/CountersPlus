using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using CountersPlus.Custom;
using IniParser;
using IniParser.Model;

namespace CountersPlus.Counters
{
    class CustomCounterHook : MonoBehaviour
    {

        private GameObject counter;
        private CustomConfigModel settings = null;
        private string Name;

        void Awake()
        {
            Name = name.Split('|').Last().Substring(1).Split(' ').First();
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            foreach (SectionData section in data.Sections)
            {
                if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                {
                    CustomConfigModel potential = new CustomConfigModel(section.SectionName);
                    if (section.Keys["SectionName"] == Name) settings = ConfigLoader.DeserializeFromConfig(potential, section.SectionName) as CustomConfigModel;
                }
            }
            if (settings == null)
            {
                Plugin.Log($"Custom Counter ({Name}) could not find its attached config model. Destroying...", Plugin.LogInfo.Notice);
                Destroy(this);
            }
            StartCoroutine(GetRequired());
        }

        IEnumerator GetRequired()
        {
            int tries = 1;
            while (true)
            {
                try
                {
                    counter = GameObject.Find(settings.Counter);
                    if (counter != null) break;
                    tries++;
                    if (tries > 10)
                    {
                        Plugin.Log($"Custom Counter ({Name}) could not find its referenced GameObject in 10 tries. Destroying...", Plugin.LogInfo.Notice);
                        Destroy(this);
                        break;
                    }
                }
                catch { }
                yield return new WaitForSeconds(tries * 0.1f);
            }
            if (tries <= 10) Init();
        }

        private void Init()
        {
            counter.transform.SetParent(transform, false);
            counter.transform.localPosition = Vector3.zero;
            Vector3 firstPosition = Vector3.zero;
            if (counter.GetComponentsInChildren<Canvas>().Any())
            {
                Canvas canvas = counter.GetComponentsInChildren<Canvas>().First();
                for (int i = 0; i < canvas.transform.childCount; i++)
                {
                    Transform child = canvas.transform.GetChild(i);
                    if (i == 0) firstPosition = child.transform.position;
                    child.transform.localPosition = ((child.transform.position - firstPosition) * TextHelper.ScaleFactor) - new Vector3(0, 0.4f, 0);
                }
            }
            transform.position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
        }
    }
}
