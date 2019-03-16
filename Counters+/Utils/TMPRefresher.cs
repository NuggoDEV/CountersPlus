using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Collections;

namespace CountersPlus
{
    class TMPRefresher : MonoBehaviour
    {
        public static TMP_FontAsset Font = null;
        private static TMPRefresher Instance;

        private static Dictionary<string, GameObject> queuedToReactivate = new Dictionary<string, GameObject>();

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(this);
        }

        public static void RefreshFont()
        {
            if (Font == null) Instance.StartCoroutine(Instance.LoadFont());
        }

        private IEnumerator LoadFont()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Any(t => t.name == "Teko-Medium SDF No Glow"));
            if (Font == null)
            {
                Font = Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF No Glow"));
                Plugin.Log("TMP Refresher | Found font asset!");
            }
        }

        public static void RefreshTMPsInGameObject(GameObject go)
        {
            if (Font == null)
            {
                Plugin.Log("TMP Refresher | Text Mesh Pro attempting visual refresh when the Font asset is not loaded.", Plugin.LogInfo.Error);
                return;
            }
            go.SetActive(false);
            string str = go.name;
            queuedToReactivate.Add(str, go);
            Instance.StartCoroutine(Instance.DelayedGameObjectReactivation(str));
        }

        private IEnumerator DelayedGameObjectReactivation(string str)
        {
            yield return new WaitForSeconds(0.1f);
            Plugin.Log($"Is Font Asset Null? {(Font == null ? "YES" : "NO")}");
            try
            {
                foreach (TextMeshPro tmp in queuedToReactivate[str].GetComponentsInChildren<TextMeshPro>()) tmp.font = Instantiate(Font);
                queuedToReactivate[str].SetActive(true);
                queuedToReactivate.Remove(str);
            }
            catch(Exception e)
            {
                Plugin.Log(e.ToString());
                Plugin.Log($"TMP Refresher | {e.GetType().Name}? My ass!", Plugin.LogInfo.Error);
            }
        }
    }
}
