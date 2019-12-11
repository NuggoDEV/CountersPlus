using IPA;
using IPA.Loader;
using IPA.Old;
using System.Collections.Generic;
using System.Linq;

namespace CountersPlus.Utils
{
    public class PluginUtility
    {
        /// <summary>
        /// Check if a BSIPA plugin is enabled
        /// </summary>
        public static bool IsPluginEnabled(string PluginName)
        {
            if (!IsPluginPresent(PluginName)) return false;
            PluginLoader.PluginInfo pluginInfo = PluginManager.GetPluginFromId(PluginName);
            if (pluginInfo?.Metadata != null) return PluginManager.IsEnabled(pluginInfo.Metadata);
            return false;
        }

        /// <summary>
        /// Check if a plugin exists
        /// </summary>
        public static bool IsPluginPresent(string PluginName)
        {
            // Check in BSIPA
            if (PluginManager.GetPlugin(PluginName) != null || PluginManager.GetPluginFromId(PluginName) != null) return true;
            #pragma warning disable CS0618 // IPA is obsolete
            return PluginManager.Plugins.Any(x => x.Name == PluginName);
            #pragma warning restore CS0618 // IPA is obsolete
        }

        /// <summary>
        /// Returns the PluginMetadata of a loaded BSIPA plugin
        /// </summary>
        public static PluginLoader.PluginMetadata GetPluginMetadata(string pluginName)
        {
            if (IsPluginPresent(pluginName))
            {
                PluginLoader.PluginMetadata metadataFromName = PluginManager.GetPlugin(pluginName).Metadata;
                PluginLoader.PluginMetadata metadataFromId = PluginManager.GetPluginFromId(pluginName).Metadata;

                if (metadataFromName != null) return metadataFromName;
                else if (metadataFromId != null) return metadataFromId;
            }
            return null;
        }

        /// <summary>
        /// Returns the PluginMetadata of a provided BSIPA plugin
        /// </summary>
        public static PluginLoader.PluginMetadata GetPluginMetadata(IPA.IBeatSaberPlugin plugin)
        {
            foreach (PluginLoader.PluginInfo pluginInfo in PluginManager.AllPlugins)
            {
                if (pluginInfo != null && plugin == pluginInfo.GetPrivateProperty<IPA.IBeatSaberPlugin>("Plugin"))
                    return pluginInfo.Metadata ?? null;
            }
            return null;
        }
    }
}
