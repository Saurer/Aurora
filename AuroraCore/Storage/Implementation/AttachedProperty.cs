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
    internal sealed class AttachedProperty<T> : Event, IAttachedProperty<T> where T : IProperty {
        private int propertyID;

        public int AttachmentID => EventValue.ID;
        public int ProviderID => EventValue.BaseEventID;
        public int PropertyID => propertyID;
        public IPropertyProvider Properties { get; private set; }

        public AttachedProperty(IDataContext context, IEventData e) : base(context, e) {
            propertyID = Int32.Parse(e.Value);
            Properties = new PropertyProvider(context, AttachmentID);
        }

        public async Task<T> GetProperty() {
            if (typeof(T) == typeof(IAttr)) {
                var attr = await Context.Storage.GetAttribute(PropertyID);
                return (T)attr;
            }
            else if (typeof(T) == typeof(IRelation)) {
                var relation = await Context.Storage.GetRelation(PropertyID);
                return (T)relation;
            }
            else {
                throw new NotImplementedException();
            }
        }

        public async Task<bool> IsRequired() {
            var constraint = await Context.Storage.GetPropertyProviderValueConstraint(ProviderID, PropertyID, StaticEvent.Required);
            if (null == constraint) {
                return Const.DefaultRequired;
            }
            else {
                return constraint.EventValue.Value == "1";
            }
        }

        public async Task<int> GetCardinality() {
            var constraint = await Context.Storage.GetPropertyProviderValueConstraint(ProviderID, PropertyID, StaticEvent.Cardinality);
            if (null == constraint) {
                return Const.DefaultCardinality;
            }
            else {
                return Int32.Parse(constraint.EventValue.Value);
            }
        }

        public async Task<int?> GetPermission() {
            var constraint = await Context.Storage.GetPropertyProviderValueConstraint(ProviderID, PropertyID, StaticEvent.Permission);
            if (null == constraint) {
                return null;
            }
            else {
                return Int32.Parse(constraint.EventValue.Value);
            }
        }

        public async Task<string> GetDefaultValue() {
            var constraint = await Context.Storage.GetPropertyProviderValueConstraint(ProviderID, PropertyID, StaticEvent.Set);
            return constraint?.EventValue.Value;
        }

        public async Task<bool> GetMutability() {
            var constraint = await Context.Storage.GetPropertyProviderValueConstraint(ProviderID, PropertyID, StaticEvent.Mutable);
            if (null == constraint) {
                return Const.DefaultMutability;
            }
            else {
                return "1" == constraint?.EventValue.Value;
            }
        }
    }
}