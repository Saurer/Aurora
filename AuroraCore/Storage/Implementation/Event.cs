// Aurora 
// Copyright (C) 2020  Frank Horrigan <https://github.com/saurer>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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