using IPA;
using IPA.Loader;
using IPA.Old;
using System.Collections.Generic;

namespace CountersPlus.Utils
{
    public class PluginUtility
    {
        /// <summary>
        /// Check if a BSIPA plugin is enabled
        /// </summary>
        public static bool IsPluginEnabled(string PluginName)
        {
            if (IsPluginPresent(PluginName))
            {
                PluginLoader.PluginInfo pluginInfo = PluginManager.GetPluginFromId(PluginName);
                if (pluginInfo?.Metadata != null)
                {
                    return PluginManager.IsEnabled(pluginInfo.Metadata);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a plugin exists
        /// </summary>
        public static bool IsPluginPresent(string PluginName)
        {
            // Check in BSIPA
            if (PluginManager.GetPlugin(PluginName) != null ||
                PluginManager.GetPluginFromId(PluginName) != null)
            {
                return true;
            }

#pragma warning disable CS0618 // IPA is obsolete
            // Check in old IPA
            foreach (IPlugin plugin in PluginManager.Plugins)
            {
                if (plugin.Name == PluginName)
                {
                    return true;
                }
            }
#pragma warning restore CS0618 // IPA is obsolete

            return false;
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

                if (metadataFromName != null)
                {
                    return metadataFromName;
                }
                else if (metadataFromId != null)
                {
                    return metadataFromId;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the PluginMetadata of a provided BSIPA plugin
        /// </summary>
        public static PluginLoader.PluginMetadata GetPluginMetadata(IBeatSaberPlugin plugin)
        {
            IEnumerable<PluginLoader.PluginInfo> pluginInfos = PluginManager.AllPlugins;
            foreach (PluginLoader.PluginInfo pluginInfo in pluginInfos)
            {
                if (pluginInfo != null &&
                    plugin == pluginInfo.GetPrivateProperty<IBeatSaberPlugin>("Plugin"))
                {
                    return pluginInfo.Metadata;
                }
            }

            return null;
        }
    }
}
