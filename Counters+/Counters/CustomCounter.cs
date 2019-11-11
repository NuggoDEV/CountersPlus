using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using CountersPlus.Custom;

namespace CountersPlus.Counters
{
    class CustomCounterHook : Counter<CustomConfigModel>
    {

        private GameObject counter;
        private string Name;

        internal override void Counter_Start()
        {
            Name = name.Split('|').Last().Substring(1).Split(' ').First();
            foreach(CustomCounter potential in CustomCounterCreator.LoadedCustomCounters)
                if (potential.SectionName == Name) settings = potential.ConfigModel;
            if (settings == null)
            {
                Plugin.Log($"Custom Counter ({Name}) could not find its attached config model. Destroying...", LogInfo.Notice);
                Destroy(this);
            }
            StartCoroutine(GetRequired());
        }

        internal override void Init(CountersData data) { }
        internal override void Counter_Destroy() { }

        IEnumerator GetRequired()
        {
            int tries = 1;
            while (true)
            {
                yield return new WaitForSeconds(tries * 0.1f);
                try
                {
                    counter = GameObject.Find(settings.CustomCounter.Counter);
                    if (counter != null) break;
                    tries++;
                    if (tries > 10)
                    {
                        Plugin.Log($"Custom Counter ({Name}) could not find its referenced GameObject in 10 tries. Destroying...", LogInfo.Notice);
                        Destroy(this);
                        break;
                    }
                }
                catch { }
            }
            if (tries <= 10) StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitUntil(() => TextHelper.CounterCanvas != null);
            counter.transform.SetParent(TextHelper.CounterCanvas.transform, false);
            counter.transform.localScale = Vector3.one;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
            position = new Vector3(position.x, position.y, 0);
            counter.transform.localPosition = position * TextHelper.ScaleFactor;
        }
    }
}
