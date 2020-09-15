using CustomJSONData;
using CustomJSONData.CustomBeatmap;
using System.Collections.Generic;

namespace CountersPlus.Counters.NoteCountProcessors
{
    /// <summary>
    /// This is a NoteCountProcessor that filters out notes that have certain CustomJSONData applied to them.
    /// </summary>
    public class CustomJSONDataNoteCountProcessor : NoteCountProcessor
    {
        // If there is any mods that implement CustomJSONData that would filter notes from the total note count,
        // PLEASE for the love of ALL THAT IS HOLY, add them to this list!!!
        // Key = CustomJSONData to search for, Value = value needed to ignore it
        private readonly Dictionary<string, bool> filteredNoteData = new Dictionary<string, bool>()
        {
            { "_fake", true }, // Noodle Extensions
        };

        public override bool ShouldIgnoreNote(NoteData data)
        {
            if (data is CustomNoteData)
            {
                dynamic customObjectData = data;
                dynamic dynData = customObjectData.customData;
                foreach (var kvp in filteredNoteData)
                {
                    bool? fake = Trees.at(dynData, kvp.Key);
                    if (fake.HasValue && fake.Value == kvp.Value)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
