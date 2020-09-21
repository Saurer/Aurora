using System;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IEvent {
        IEventData EventValue { get; }
        DateTime Date { get; }
        ConditionsContainer Conditions { get; }
        Task<IIndividual> GetCreator();
        Task<IEvent> GetConditionEvent();
        Task<IEvent> GetBaseEvent();
        Task<IEvent> GetValueTypeEvent();
    }
}