using IPA.Loader;
using SemVer;
using SimpleJSON;
using System.Collections;
using UnityEngine.Networking;

namespace CountersPlus.Utils
{
    public class VersionUtility
    {
        public Version PluginVersion { get; private set; } = new Version("0.0.0");
        public Version BeatModsVersion { get; private set; } = new Version("0.0.0");
        public bool HasLatestVersion => PluginVersion >= BeatModsVersion;

        public VersionUtility()
        {
            // I could grab this straight from PluginMetadata but this is for cleanness.
            PluginVersion = PluginManager.GetPlugin("Counters+").Version;

            SharedCoroutineStarter.instance.StartCoroutine(GetBeatModsVersion());
        }

        private IEnumerator GetBeatModsVersion()
        {
            using (UnityWebRequest www = UnityWebRequest.Get("https://beatmods.com/api/v1/mod?search=Counters%2B"))
            {
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Plugin.Logger.Error("Failed to download version info.");
                    yield break;
                }
                JSONNode node = JSON.Parse(www.downloadHandler.text);
                foreach (JSONNode child in node)
                {
                    if (child["status"] != "approved") continue;
                    BeatModsVersion = new Version(child["version"].Value);
                    break;
                }
            }
            if (!HasLatestVersion) Plugin.Logger.Warn("Uh oh! We aren't up to date!");
        }
    }
}
