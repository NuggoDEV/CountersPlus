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
            PluginMetadata pluginInfo = PluginManager.GetPluginFromId(PluginName);
            if (pluginInfo != null) return PluginManager.IsEnabled(pluginInfo);
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
        public static PluginMetadata GetPluginMetadata(string pluginName)
        {
            if (IsPluginPresent(pluginName))
            {
                PluginMetadata metadataFromName = PluginManager.GetPlugin(pluginName);
                PluginMetadata metadataFromId = PluginManager.GetPluginFromId(pluginName);

                if (metadataFromName != null) return metadataFromName;
                else if (metadataFromId != null) return metadataFromId;
            }
            return null;
        }
    }
}
