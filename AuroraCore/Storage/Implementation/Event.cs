using System;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal class Event : IEvent {
        protected IDataContext Context { get; private set; }

        public IEventData EventValue { get; private set; }
        public DateTime Date => EventValue.Date;
        public ConditionRule Conditions { get; private set; }

        public Event(IDataContext context, IEventData e) {
            Context = context;
            EventValue = e;
            Conditions = e.Conditions;
        }

        public async Task<IIndividual> GetCreator() =>
            await Context.Storage.GetActor(EventValue.ActorEventID);

        public async Task<IEvent> GetConditionEvent() {
            if (Conditions is ConditionRule.EventConditionRule eventCondition) {
                return await Context.Storage.GetEvent(eventCondition.EventID);
            }
            else {
                return null;
            }
        }

        public async Task<IEvent> GetBaseEvent() =>
            await Context.Storage.GetEvent(EventValue.BaseEventID);

        public async Task<IEvent> GetValueTypeEvent() =>
            await Context.Storage.GetEvent(EventValue.ValueID);
    }
}