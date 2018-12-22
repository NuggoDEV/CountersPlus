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
using Newtonsoft.Json;

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
            if (!Directory.Exists(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters"))
            {
                Plugin.Log($"Custom Counter ({Name}) could not find its attached config model. Destroying...");
                Destroy(this);
            }
            else
            {
                foreach(string file in Directory.EnumerateFiles(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters"))
                {
                    CustomConfigModel potential = JsonConvert.DeserializeObject<CustomConfigModel>(File.ReadAllText(file));
                    if (potential.JSONName == Name) settings = potential;
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
        }

        void Update()
        {
            if (CountersController.rng)
            {
                settings.Index = UnityEngine.Random.Range(0, 5);
                settings.Position = (ICounterPositions)UnityEngine.Random.Range(0, 4);
            }
            else
            {
                if (CountersController.settings.RNG)
                {
                    transform.position = Vector3.Lerp(
                    transform.position,
                    CountersController.determinePosition(gameObject, settings.Position, settings.Index),
                    Time.deltaTime);
                }
                else
                    transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            }

        }
    }
}
