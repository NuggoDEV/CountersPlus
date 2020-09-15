using System.Collections.Generic;
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


        [Inject] private BeatmapData beatmapData;

        protected List<NoteData> GetNoteData(BeatmapData data)
        {
            List<NoteData> allNoteData = new List<NoteData>();
            BeatmapLineData[] beatmapLinesData = data.beatmapLinesData;
            for (int i = 0; i < beatmapLinesData.Length; i++)
            {
                foreach (BeatmapObjectData beatmapObjectData in beatmapLinesData[i].beatmapObjectsData)
                {
                    if (beatmapObjectData is NoteData note && note.noteType.IsBasicNote())
                    {
                        if (ShouldIgnoreNote(note))
                            continue;
                        else
                            allNoteData.Add(note);
                    }
                }
            }

            return allNoteData;
        }

        public abstract bool ShouldIgnoreNote(NoteData data);
    }
}
