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

using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal class Individual : Event, IIndividual {
        public int IndividualID => EventValue.ID;
        public int EventBase => EventValue.BaseEventID;
        public int ModelID {
            get {
                return (Conditions as ConditionRule.EventConditionRule).EventID;
            }
        }
        public string Label => EventValue.Value;
        public IPropertyContainer Properties { get; private set; }

        public Individual(IDataContext context, IEventData e) : base(context, e) {
            Properties = new PropertyContainer(context, ModelID, IndividualID);
        }

        public async Task<IModel> GetModel() =>
            await Context.Storage.GetModel(ModelID);
    }
}