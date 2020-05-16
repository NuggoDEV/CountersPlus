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
        internal static Dictionary<CounterWarning, TMP_Text> existing = new Dictionary<CounterWarning, TMP_Text>();
        private int warningOrder = 0;
        private float persistTime;
        TMP_Text tmpro;

        public static void Create(string text, float persistTimeInSeconds = 5)
        {
            CounterWarning newWarning = new GameObject("Counters+ | Warning").AddComponent<CounterWarning>();
            newWarning.warningText = text;
            newWarning.persistTime = persistTimeInSeconds - 0.5f;
            newWarning.warningOrder = warnings;
            warnings++;
        }

        public static void ClearAllWarnings()
        {
            foreach (TMP_Text warning in existing.Values) Destroy(warning.gameObject);
            foreach (CounterWarning warning in existing.Keys) Destroy(warning.gameObject);
            warnings = 0;
            existing.Clear();
            if (warningsCanvas != null) Destroy(warningsCanvas.gameObject);
            warningsCanvas = null;
        }

        void Start()
        {
            if (warningsCanvas == null) warningsCanvas = TextHelper.CreateCanvas(Vector3.forward * 2.25f, true);
            Vector3 position = new Vector3(0, 2.1f - (warningOrder * 0.1f), 0);
            TextHelper.CreateText(out tmpro, warningsCanvas, position);
            tmpro.fontSize = 1;
            tmpro.color = Color.white;
            tmpro.alignment = TextAlignmentOptions.Center;
            tmpro.text = $"<u>{warningText}</u>";
            existing.Add(this, tmpro);
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
