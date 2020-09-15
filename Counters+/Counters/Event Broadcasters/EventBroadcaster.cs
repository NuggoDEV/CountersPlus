using CountersPlus.Counters.Interfaces;
using System.Collections.Generic;
using System;
using Zenject;

namespace CountersPlus.Counters.Event_Broadcasters
{
    /// <summary>
    /// An abstract class that broadcasts in-game events across Counters that inherit <see cref="IEventHandler"/>s.
    /// This was designed such that changes to these in-game events will only break the Event Broadcasters.
    /// </summary>
    /// <typeparam name="T">Event handler that will receive these broadcasts.</typeparam>
    internal abstract class EventBroadcaster<T>: IInitializable, IDisposable where T : IEventHandler
    {
        [Inject] protected List<T> EventHandlers = new List<T>();

        public abstract void Initialize();

        public abstract void Dispose();
    }
}
