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

namespace AuroraCore.Storage.Implementation {
    internal sealed class Entity : Event, IEntity {
        public int EntityID => EventValue.ID;

        public string Label => EventValue.Value;

        public Entity(IDataContext context, IEventData e) : base(context, e) {

        }

        public async Task<IEnumerable<IModel>> GetModels() =>
            await Context.Storage.GetEntityModels(EntityID);

        public async Task<IEnumerable<IIndividual>> GetIndividuals() =>
            await Context.Storage.GetEntityIndividuals(EntityID);
    }
}