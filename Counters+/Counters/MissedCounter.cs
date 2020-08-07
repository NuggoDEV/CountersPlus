using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;

namespace CountersPlus.Counters
{
    public class MissedCounter : Counter<MissedConfigModel>, INoteEventHandler
    {
        private int notesMissed = 0;

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (data.noteType != NoteType.Bomb) return;
            notesMissed++;
        }

        public void OnNoteMiss(NoteData data)
        {
            if (data.noteType == NoteType.Bomb) return;
            notesMissed++;
        }
    }
}
