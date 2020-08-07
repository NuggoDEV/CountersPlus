using CountersPlus.Counters.Interfaces;
using System.Collections.Generic;
using System;
using Zenject;

namespace CountersPlus.Counters.Event_Broadcasters
{
    public abstract class EventBroadcaster<T>: IInitializable, IDisposable where T : IEventHandler
    {
        [Inject] protected List<T> EventHandlers = new List<T>();

        public abstract void Initialize();

        public abstract void Dispose();
    }
}
