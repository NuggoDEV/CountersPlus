using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using IPA.Loader;

namespace CountersPlus
{
    class VersionChecker : MonoBehaviour
    {
        private static VersionChecker _instance;
        public static VersionChecker Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new GameObject("Counters+ | Version Checker").AddComponent<VersionChecker>();
                DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }

        public static void GetOnlineVersion()
        {
            Instance.StartCoroutine(GetOnlineVersionRoutine());
        }

        private static IEnumerator GetOnlineVersionRoutine()
        {
            Plugin.Log("Obtaining latest version information...", Plugin.LogInfo.Notice);
            using (UnityWebRequest www = UnityWebRequest.Get("https://beatmods.com/api/v1/mod?search=Counters%2B"))
            {
                yield return (www.SendWebRequest());
                if (www.isHttpError || www.isNetworkError) Plugin.Log("Failed to download version info.", Plugin.LogInfo.Error, "Check your internet connection, or the BeatMods website.");
                else
                {
                    Plugin.Log("Obtained latest version info!", Plugin.LogInfo.Notice);
                    JSONNode node = JSON.Parse(www.downloadHandler.text);
                    foreach(JSONNode child in node.Children)
                    {
                        if (child["status"] != "approved") continue;
                        try
                        {
                            string version = child["version"].Value;
                            Plugin.upToDate = isLatestVersion(version);
                            Plugin.webVersion = version;
                            break;
                        } catch { }
                    }
                }
            }
        }

        private static bool isLatestVersion(string downloadedVersion)
        {
            string Version = PluginManager.GetPlugin("Counters+").Metadata.Version.ToString();
            List<int> pluginVersion = new List<int>();
            List<int> webVersion = new List<int>();
            foreach(string num in Version.Split('.'))
                pluginVersion.Add(int.Parse(num));
            foreach (string num in downloadedVersion.Split('.'))
                webVersion.Add(int.Parse(num));
            for (int i = 0; i < pluginVersion.Count(); i++)
            {  //Also checks the number (Major or Minor) before it (So 1.2.5 doesnt get seen as newer than 1.3.3)
                if (i > 0)
                    if (pluginVersion[i] > webVersion[i] && pluginVersion[i - 1] > webVersion[i - 1]) return true;
                    else if (pluginVersion[i] < webVersion[i]) return false;
                else if (pluginVersion[i] > webVersion[i]) return true;
            }
            if (Version == downloadedVersion) return true;
            return false;
        }
    }
}
