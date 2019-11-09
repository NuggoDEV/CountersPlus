using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CountersPlus.UI
{
    class CounterWarning : MonoBehaviour
    {
        private string warningText = "";
        private static int warnings = 0;
        private static Canvas warningsCanvas;
        internal static List<CounterWarning> existing = new List<CounterWarning>();
        private int warningOrder = 0;
        private float persistTime;
        TMP_Text tmpro;

        public static void CreateWarning(string text, float persistTimeInSeconds)
        {
            CounterWarning newWarning = new GameObject("Counters+ | Warning").AddComponent<CounterWarning>();
            newWarning.warningText = text;
            newWarning.persistTime = persistTimeInSeconds - 0.5f;
            newWarning.warningOrder = warnings;
            existing.Add(newWarning);
            warnings++;
        }

        public static void ClearAllWarnings()
        {
            foreach (CounterWarning warning in existing) Destroy(warning.gameObject);
            warnings = 0;
            existing.Clear();
        }

        void Start()
        {
            if (warningsCanvas == null) warningsCanvas = TextHelper.CreateCanvas(Vector3.forward * 2.25f);
            Vector3 position = new Vector3(0, 2.1f - (warningOrder * 0.1f), 0);
            TextHelper.CreateText(out tmpro, warningsCanvas, position);
            tmpro.fontSize = 1;
            tmpro.color = Color.white;
            tmpro.alignment = TextAlignmentOptions.Center;
            tmpro.text = $"<u>{warningText}</u>";
            StartCoroutine(PersistALittleBit());
        }

        IEnumerator PersistALittleBit()
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(KillThyself());
        }

        IEnumerator KillThyself()
        {
            yield return new WaitForSeconds(persistTime);
            existing.Remove(this);
            Destroy(tmpro);
            Destroy(gameObject);
            warnings--;
        }
    }
}
