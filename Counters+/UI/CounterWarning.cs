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
        private static int warnings = 0;
        internal static List<CounterWarning> existing = new List<CounterWarning>();
        private string warningText = "";
        private int warningOrder = 0;
        private float persistTime;
        TextMeshPro tmpro;

        public static void CreateWarning(string text, float persistTimeInSeconds)
        {
            CounterWarning newWarning = new GameObject("Counters+ | Warning").AddComponent<CounterWarning>();
            GameObject go = newWarning.gameObject;
            newWarning.UpdateText(text);
            newWarning.persistTime = persistTimeInSeconds - 0.5f;
            newWarning.warningOrder = warnings;
            existing.Add(newWarning);
            warnings++;
            TMPRefresher.RefreshTMPsInGameObject(go);
        }

        void Awake()
        {
            tmpro = gameObject.AddComponent<TextMeshPro>();
            tmpro.fontSize = 1;
            tmpro.color = Color.white;
            tmpro.alignment = TextAlignmentOptions.Center;
            StartCoroutine(PersistALittleBit());
        }

        void UpdateText(string newWarning)
        {
            warningText = newWarning;
        }

        void Update()
        {
            tmpro.text = $"<u>{warningText}</u>";
            tmpro.rectTransform.position = new Vector3(0, 2.1f - (warningOrder * 0.1f), 2.25f);
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
            Destroy(gameObject);
            warnings--;
        }
    }
}
