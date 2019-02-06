using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

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
            Plugin.Log("Obtaining latest version information...");
            string url = "https://modsaber.org/api/v1.1/mods/versions/countersplus";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return (www.SendWebRequest());
                if (www.isHttpError || www.isNetworkError)
                {
                    Plugin.Log("Failed to download version info.", Plugin.LogInfo.Warning);
                }
                else
                {
                    Plugin.Log("Obtained latest version info!");
                    JSONNode node = JSON.Parse(www.downloadHandler.text);
                    IEnumerable<JSONNode> nodes = node.Children;
                    using (IEnumerator<JSONNode> enumerator = nodes.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current["approval"]["status"] == "approved")
                            {
                                try
                                {
                                    string version = enumerator.Current["version"].Value;
                                    Plugin.upToDate = isLatestVersion(version);
                                    Plugin.webVersion = version;
                                    if (isLatestVersion(version))
                                        Plugin.Log("We're running on the latest version!");
                                    else
                                        Plugin.Log("We're on an outdated build!");
                                    break;
                                }
                                catch
                                {
                                    Plugin.Log("Could not parse version info!", Plugin.LogInfo.Error);
                                    Plugin.webVersion = "BAD VERSION";
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool isLatestVersion(string downloadedVersion)
        {
            List<int> pluginVersion = new List<int>();
            List<int> webVersion = new List<int>();
            foreach(string num in Plugin.Instance.Version.Split('.'))
            {
                string parse = num;
                if (num.Contains("-")) parse = num.Split('-').First();
                pluginVersion.Add(int.Parse(parse));
            }
            foreach (string num in downloadedVersion.Split('.'))
            {
                webVersion.Add(int.Parse(num));
            }
            for (int i = 0; i < pluginVersion.Count(); i++)
            {
                if (pluginVersion[i] > webVersion[i]) return true;
            }
            if (Plugin.Instance.Version == downloadedVersion) return true;
            return false;
        }
    }
}
