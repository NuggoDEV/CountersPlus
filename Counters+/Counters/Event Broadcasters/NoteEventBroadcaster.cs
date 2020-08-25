using CountersPlus.Counters.Interfaces;
using Zenject;

namespace CountersPlus.Counters.Event_Broadcasters
{
    /// <summary>
    /// A <see cref="EventBroadcaster{T}"/> that broadcasts events relating to note cutting and missing.
    /// </summary>
    internal class NoteEventBroadcaster : EventBroadcaster<INoteEventHandler>
    {
        [Inject] private ScoreController scoreController;

        public override void Initialize()
        {
            scoreController.noteWasCutEvent += NoteWasCutEvent;
            scoreController.noteWasMissedEvent += NoteWasMissedEvent;
        }

        private void NoteWasCutEvent(NoteData data, NoteCutInfo noteCutInfo, int _)
        {
            foreach (INoteEventHandler noteEventHandler in EventHandlers)
            {
                noteEventHandler?.OnNoteCut(data, noteCutInfo);
            }
        }

        private void NoteWasMissedEvent(NoteData data, int _)
        {
            foreach (INoteEventHandler noteEventHandler in EventHandlers)
            {
                noteEventHandler?.OnNoteMiss(data);
            }
        }

        public override void Dispose()
        {
            scoreController.noteWasCutEvent -= NoteWasCutEvent;
            scoreController.noteWasMissedEvent -= NoteWasMissedEvent;
        }
    }
}
