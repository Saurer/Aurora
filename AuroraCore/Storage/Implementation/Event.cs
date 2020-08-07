using System;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal class Event : IEvent {
        protected IDataContext Context { get; private set; }

        public IEventData EventValue { get; private set; }
        public DateTime Date => EventValue.Date;

        public Event(IDataContext context, IEventData e) {
            Context = context;
            EventValue = e;
        }

        public async Task<IIndividual> GetCreator() =>
            await Context.Storage.GetActor(EventValue.ActorEventID);

        public async Task<IEvent> GetConditionEvent() =>
            await Context.Storage.GetEvent(EventValue.ConditionEventID);

        public async Task<IEvent> GetBaseEvent() =>
            await Context.Storage.GetEvent(EventValue.BaseEventID);

        public async Task<IEvent> GetValueTypeEvent() =>
            await Context.Storage.GetEvent(EventValue.ValueID);
    }
}