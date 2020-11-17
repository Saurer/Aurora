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

using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraCore.Types;

namespace AuroraCore.Storage.Implementation {
    internal class Attr : Event, IAttr {
        public int PropertyID => EventValue.ID;
        public string Label => EventValue.Value;

        public Attr(IDataContext context, IEventData e) : base(context, e) {
        }

        public async Task<bool> IsBoxed() {
            var dt = await GetDataType();
            return dt.IsBoxed;
        }

        public async Task<DataType> GetDataType() {
            var constraint = await Context.Storage.GetAttributeAttachedConstraint(PropertyID, StaticEvent.DataType);

            if (null == constraint) {
                return null;
            }

            var constraintValue = await constraint.GetValue();
            if (null == constraintValue) {
                return null;
            }

            return Context.Types.Get(constraintValue.Label);
        }

        public async Task<IAttachedAttrConstraint> GetConstraint(int constraintID) =>
            await Context.Storage.GetAttributeAttachedConstraint(PropertyID, constraintID);

        public async Task<IEnumerable<IAttachedAttrConstraint>> GetConstraints() =>
            await Context.Storage.GetAttributeAttachedConstraints(PropertyID);

        public async Task<IEnumerable<IEvent>> GetValueCandidates() =>
            await Context.Storage.GetAttributeValueCandidates(PropertyID);

        public async Task<bool> Validate(string value) {
            var dataType = await GetDataType();
            return dataType.Validate(value);
        }
    }
}