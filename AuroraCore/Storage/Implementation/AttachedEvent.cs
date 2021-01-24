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
    internal sealed class AttachedEvent : Event, IAttachedEvent<IProperty> {
        public int AttachmentID => EventValue.ID;
        public int ProviderID => EventValue.BaseEventID;
        public int PropertyID => EventValue.ValueID;
        public string Value => EventValue.Value;

        public AttachedEvent(IDataContext context, IEventData e) : base(context, e) {
        }

        public async Task<IProperty> GetProperty() {
            var eventBase = await Context.Storage.GetEvent(PropertyID);

            if (eventBase.EventValue.BaseEventID == StaticEvent.Attribute) {
                var attr = await Context.Storage.GetAttribute(PropertyID);
                return attr;
            }
            else if (eventBase.EventValue.BaseEventID == StaticEvent.Relation) {
                var attr = await Context.Storage.GetRelation(PropertyID);
                return attr;
            }
            else {
                throw new NotImplementedException();
            }
        }
    }
}