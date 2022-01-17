using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Heck.SettingsSetter;
using IPA.Config.Stores.Attributes;
using static CountersPlus.Utils.EnumerableExtensions;

namespace CountersPlus.ConfigModels.SettableSettings
{
    internal class CountersPlusSettableSettings : IDisposable
    {
        public static bool HasExecutedBefore { get; private set; } = false;

        private const string countersPlusIdentifier = "_countersPlus";

        private readonly BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        private static List<ISettableSetting> settableSettings = new();

        // ALRIGHT this is going to be a dousy
        // We have hundreds of settings to expose to Heck, aint no way in heck (haha) that I'm manually registering all of them.
        // Time to Reflection this bitch!
        public CountersPlusSettableSettings(List<ConfigModel> configs, MainConfigModel mainConfigModel, HUDConfigModel hudConfig)
        {
            if (HasExecutedBefore) return;

            HasExecutedBefore = true;

            // Grab all of the objects i'll make configurable to the mapper
            // (All counter config models, plus main settings and main canvas)
            // and ensure only one of each is in the collection
            var configurableObjects = configs
                .DistinctBy(x => x.DisplayName) // DistinctBy LINQ method might not be needed, zenject supposedly doesn't return duplicate objects but it currently acts as a safe-guard.
                .Cast<object>()
                .Append(mainConfigModel)
                .Append(hudConfig.MainCanvasSettings);

            // Caching these before the loop
            var settableSettingType = typeof(CountersPlusWrapperSetting);
            var ignoreAttributeType = typeof(IgnoreAttribute);

            // Iterate through all of the settings
            foreach (var configurableObj in configurableObjects)
            {
                var type = configurableObj.GetType();

                // Grab their display name
                var displayNameType = type.GetProperty("DisplayName", bindingFlags) ?? type.GetProperty("Name", bindingFlags);
                var displayName = displayNameType.GetValue(configurableObj) as string;

                // Grab the properties I want to register
                var applicableProperties = type
                    .GetProperties(bindingFlags)
                    .Where(x =>
                        // Filter types assigned by BSIPA Config (virtual read/write properties)
                        x.CanWrite && x.CanRead && x.GetMethod.IsVirtual
                        // as well as types that Heck supports (structs/value types)
                        && x.PropertyType.IsValueType
                        // and types that arent ignored by de/serialization.
                        && x.GetCustomAttribute(ignoreAttributeType) == null
                        // and I guess not ending with #? Might be a BSIPA config thing.
                        && !x.Name.EndsWith("#"));

                // Iterate over these properties
                foreach (var applicableProperty in applicableProperties)
                {
                    var propertyName = applicableProperty.Name;

                    // Dynamically create settable setting object
                    var settableSetting = Activator.CreateInstance(settableSettingType,
                        $"Counters+ | {displayName}", propertyName,
                        applicableProperty, configurableObj) as ISettableSetting;

                    settableSettings.Add(settableSetting);

                    var fieldName = GetFieldName(displayName, propertyName);

                    // Haha register
                    SettingSetterSettableSettingsManager.RegisterSettableSetting(countersPlusIdentifier, fieldName, settableSetting);

                    //Plugin.Logger.Debug($"Registered {fieldName}.");
                }
            }

            Plugin.Logger.Notice($"Registered {settableSettings.Count} settings to Heck's settable settings system.");
        }

        public void Dispose()
        {
            settableSettings.ForEach(x => x.SetTemporary(null));
        }

        private string GetFieldName(string displayName, string propertyName)
            => $"_{CleanUpString(displayName.ToLowerInvariant())}{CleanUpString(propertyName)}";

        private string CleanUpString(string original)
        {
            var builder = new StringBuilder();

            var nextCharacterUppercase = false;

            foreach (var ch in original)
            {
                if (ch == '_' || ch == '#') continue;

                if (ch == ' ')
                {
                    nextCharacterUppercase = true;
                    continue;
                }

                if (nextCharacterUppercase)
                {
                    builder.Append(ch.ToString().ToUpperInvariant());
                    nextCharacterUppercase = false;
                    continue;
                }

                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}
