using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using CountersPlus.Custom;
using System.IO;
using IniParser;
using IniParser.Model;

namespace CountersPlus.Counters
{
    class CustomCounterHook : MonoBehaviour
    {

        private GameObject counter;
        private CustomConfigModel settings;
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
                    if (potential.SectionName == Name) settings = potential;
                }
            }
            if (settings == null)
            {
                Plugin.Log($"Custom Counter ({Name}) could not find its attached config model. Destroying...");
                Destroy(this);
            }
            StartCoroutine(GetRequired());
        }

        IEnumerator GetRequired()
        {
            int tries = 1;
            while (true)
            {
                counter = GameObject.Find(settings.Counter);
                if (counter != null) break;
                tries++;
                if (tries > 10)
                {
                    Plugin.Log($"Custom Counter ({Name}) could not find its referenced GameObject in 10 tries. Destroying...");
                    Destroy(this);
                    break;
                }
                yield return new WaitForSeconds(tries * 0.1f);
            }
            if (tries <= 10) Init();
        }

        private void Init()
        {
            counter.transform.parent = transform;
            counter.transform.localPosition = Vector3.zero;
            Vector3 firstPosition = Vector3.zero;
            if (counter.GetComponentsInChildren<TMP_Text>().Any())
            {
                for (int i = 0; i < counter.GetComponentsInChildren<TMP_Text>().Reverse().Count(); i++)
                {
                    TMP_Text tmp = counter.GetComponentsInChildren<TMP_Text>().Reverse().ToArray()[i];
                    if (i == 0) firstPosition = tmp.transform.position;
                    tmp.transform.localPosition = ((tmp.transform.position - firstPosition) * TextHelper.ScaleFactor) - new Vector3(0, 0.4f, 0);
                }
            }
            transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
        }
    }
}
