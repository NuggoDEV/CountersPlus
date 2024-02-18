using IPA.Loader;
using IPA.Loader.Features;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace CountersPlus.Custom
{
    public class CustomCounterFeature : Feature
    {
        private Dictionary<PluginMetadata, CustomCounter> incompleteCustomCounters = new Dictionary<PluginMetadata, CustomCounter>(0);

        protected override bool Initialize(PluginMetadata meta, JObject featureData)
        {
            CustomCounter counter;
            try
            {
                counter = featureData.ToObject<CustomCounter>();
            }
            catch (Exception e)
            {
                InvalidMessage = $"Invalid data: {e}";
                return false;
            }

            incompleteCustomCounters.Add(meta, counter);
            return true;
        }

        public override void AfterInit(PluginMetadata meta)
        {
            if (incompleteCustomCounters.TryGetValue(meta, out CustomCounter counter))
            {
                if (!TryLoadType(ref counter.CounterType, meta, counter.CounterLocation))
                {
                    Plugin.Logger.Error($"Failed to load a Type from the provided CounterLocation for {counter.Name}.");
                    return;
                }
                if (counter.BSML != null && counter.BSML.HasType && !TryLoadType(ref counter.BSML.HostType, meta, counter.BSML.Host))
                {
                    Plugin.Logger.Error($"Failed to load a Type from the provided BSML Host for {counter.Name}.");
                    return;
                }

                Plugin.LoadedCustomCounters.Add(counter);
                Plugin.Logger.Notice($"Loaded a Custom Counter ({counter.Name}).");
            }
            else
            {
                Plugin.Logger.Critical(@"A plugin has a defined Custom Counter, but Initialise was somehow not called. 
                                         How the hell did we even get here?");
            }
        }

        private bool TryLoadType(ref Type typeToLoad, PluginMetadata meta, string location)
        {
            // totally didn't yoink this from BSIPA's ConfigProviderFeature
            try
            {
                typeToLoad = meta.Assembly.GetType(location);
            }
            catch (ArgumentException)
            {
                InvalidMessage = $"Invalid type name {location}";
                return false;
            }
            catch (Exception e) when (e is FileNotFoundException || e is FileLoadException || e is BadImageFormatException)
            {
                string filename;

                switch (e)
                {
                    case FileNotFoundException fn:
                        filename = fn.FileName;
                        goto hasFilename;
                    case FileLoadException fl:
                        filename = fl.FileName;
                        goto hasFilename;
                    case BadImageFormatException bi:
                        filename = bi.FileName;
                    hasFilename:
                        InvalidMessage = $"Could not find {filename} while loading type";
                        break;
                    default:
                        InvalidMessage = $"Error while loading type: {e}";
                        break;
                }

                return false;
            }
            catch (Exception e) // Is this unnecessary? Maybe.
            {
                InvalidMessage = $"An unknown error occured: {e}";
                return false;
            }

            return true;
        }
    }
}