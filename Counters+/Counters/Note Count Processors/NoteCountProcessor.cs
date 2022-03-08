using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace CountersPlus.Counters.NoteCountProcessors
{
    /// <summary>
    /// Helper class to correctly obtain all notes in a song.
    /// This should handle the Vanilla game (all notes are valid), and modded (some notes are not valid).
    /// 
    /// Perhaps in the future, these could be expanded upon to filter notes on miss/cut events as well.
    /// </summary>
    public abstract class NoteCountProcessor
    {
        public List<NoteData> Data
        {
            get
            {
                if (data is null) data = GetNoteData(beatmapData);
                return data;
            }
        }

        public int NoteCount => Data.Count;

        private List<NoteData> data;


        [Inject] private IReadonlyBeatmapData beatmapData;

        protected List<NoteData> GetNoteData(IReadonlyBeatmapData data)
        {
            List<NoteData> allNoteData = new List<NoteData>();
            foreach (var beatmapObjectData in data.allBeatmapDataItems)
            {
                if (beatmapObjectData is NoteData note && note.colorType != ColorType.None)
                {
                    if (ShouldIgnoreNote(note))
                        continue;
                    else
                        allNoteData.Add(note);
                }
            }

            return allNoteData;
        }

        public abstract bool ShouldIgnoreNote(NoteData data);
    }
}
