using CountersPlus.Counters.Interfaces;
using Zenject;

namespace CountersPlus.Counters.Event_Broadcasters
{
    /// <summary>
    /// A <see cref="EventBroadcaster{T}"/> that broadcasts events relating to note cutting and missing.
    /// </summary>
    internal class NoteEventBroadcaster : EventBroadcaster<INoteEventHandler>
    {
        [Inject] private BeatmapObjectManager scoreController;

        public override void Initialize()
        {
            scoreController.noteWasCutEvent += NoteWasCutEvent;
            scoreController.noteWasMissedEvent += NoteWasMissedEvent;
        }

        private void NoteWasCutEvent(NoteController data, NoteCutInfo noteCutInfo)
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
            scoreController.noteWasCutEvent -= NoteWasCutEvent;
            scoreController.noteWasMissedEvent -= NoteWasMissedEvent;
        }
    }
}
