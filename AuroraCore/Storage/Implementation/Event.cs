using System;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal class Event : IEvent {
        protected IDataContext Context { get; private set; }

        public IEventData EventValue { get; private set; }
        public DateTime Date => EventValue.Date;
        public ConditionsContainer Conditions { get; private set; }

        public Event(IDataContext context, IEventData e) {
            Context = context;
            EventValue = e;
            Conditions = new ConditionsContainer(e.Conditions);
        }

        public async Task<IIndividual> GetCreator() =>
            await Context.Storage.GetActor(EventValue.ActorEventID);

        public async Task<IEvent> GetConditionEvent() {
            var eventCondition = Conditions.TryGetEvent();
            if (null == eventCondition) {
                return null;
            }

            return await Context.Storage.GetEvent(eventCondition.EventID);
        }

        public async Task<IEvent> GetBaseEvent() =>
            await Context.Storage.GetEvent(EventValue.BaseEventID);

        public async Task<IEvent> GetValueTypeEvent() =>
            await Context.Storage.GetEvent(EventValue.ValueID);
    }
}