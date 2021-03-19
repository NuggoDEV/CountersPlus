using CountersPlus.Counters.Interfaces;
using Zenject;

namespace CountersPlus.Counters.Event_Broadcasters
{
    /// <summary>
    /// A <see cref="EventBroadcaster{T}"/> that broadcasts events relating to note cutting and missing.
    /// </summary>
    internal class NoteEventBroadcaster : EventBroadcaster<INoteEventHandler>
    {
        [Inject] private BeatmapObjectManager beatmapObjectManager;

        public override void Initialize()
        {
            beatmapObjectManager.noteWasCutEvent += NoteWasCutEvent;
            beatmapObjectManager.noteWasMissedEvent += NoteWasMissedEvent;
        }

        private void NoteWasCutEvent(NoteController data, in NoteCutInfo noteCutInfo)
        {
            foreach (INoteEventHandler noteEventHandler in EventHandlers)
            {
                noteEventHandler?.OnNoteCut(data.noteData, noteCutInfo);
            }
        }

        private void NoteWasMissedEvent(NoteController data)
        {
            foreach (INoteEventHandler noteEventHandler in EventHandlers)
            {
                noteEventHandler?.OnNoteMiss(data.noteData);
            }
        }

        public override void Dispose()
        {
            beatmapObjectManager.noteWasCutEvent -= NoteWasCutEvent;
            beatmapObjectManager.noteWasMissedEvent -= NoteWasMissedEvent;
        }
    }
}
