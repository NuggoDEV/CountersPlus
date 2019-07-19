using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using IPA.Loader;
using SemVer;

namespace CountersPlus
{
    class VersionChecker : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(GetOnlineVersionRoutine());
        }

        private static IEnumerator GetOnlineVersionRoutine()
        {
            Plugin.Log("Obtaining latest version information...", Plugin.LogInfo.Notice);
            using (UnityWebRequest www = UnityWebRequest.Get("https://beatmods.com/api/v1/mod?search=Counters%2B"))
            {
                yield return (www.SendWebRequest());
                if (www.isHttpError || www.isNetworkError)
                {
                    Plugin.Log("Failed to download version info.", Plugin.LogInfo.Error, "Check your internet connection, or the BeatMods website.");
                    yield break;
                }
                Plugin.Log("Obtained latest version info!", Plugin.LogInfo.Notice);
                JSONNode node = JSON.Parse(www.downloadHandler.text);
                foreach (JSONNode child in node.Children)
                {
                    if (child["status"] != "approved") continue;
                    Plugin.webVersion = child["version"].Value;
                    Plugin.upToDate = IsLatestVersion(Plugin.webVersion);
                    break;
                }
            }
            if (!Plugin.upToDate) Plugin.Log("Uh oh! We aren't up to date!", Plugin.LogInfo.Warning);
        }

        private static bool IsLatestVersion(string downloadedVersion)
        {
            Version client = PluginManager.GetPlugin("Counters+").Metadata.Version;
            Version server = new Version(downloadedVersion);
            return client >= server;
        }
    }
}
