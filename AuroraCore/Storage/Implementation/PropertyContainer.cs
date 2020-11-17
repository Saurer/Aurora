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
using System.Linq;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal class PropertyContainer : IPropertyContainer {
        private readonly int containerID;
        private readonly IPropertyProvider provider;
        private readonly IDataContext context;

        public PropertyContainer(IDataContext context, int providerID, int containerID) {
            this.context = context;
            this.provider = new PropertyProvider(context, providerID);
            this.containerID = containerID;
        }

        public async Task<IEnumerable<IBoxedValue>> GetAttribute(int attributeID) =>
            await context.Storage.GetPropertyContainerAttribute(containerID, attributeID);

        public async Task<IEnumerable<IBoxedValue>> GetRelation(int relationID) =>
            await context.Storage.GetPropertyContainerRelation(containerID, relationID);

        public async Task<IReadOnlyDictionary<int, IEnumerable<IBoxedValue>>> GetAttributes() =>
            await context.Storage.GetPropertyContainerAttributes(containerID);

        public async Task<IReadOnlyDictionary<int, IEnumerable<IBoxedValue>>> GetRelations() =>
            await context.Storage.GetPropertyContainerRelations(containerID);

        public async Task<bool> Validate() {
            var attributes = await provider.GetAttributes();
            var relations = await provider.GetRelations();

            var attributeValues = await GetAttributes();
            foreach (var attribute in attributes) {
                var required = await attribute.IsRequired();

                if (!required) {
                    continue;
                }

                if (attributeValues.TryGetValue(attribute.PropertyID, out var values) && values.Any()) {
                    continue;
                }
                else {
                    return false;
                }
            }

            var relationValues = await GetRelations();
            foreach (var relation in relations) {
                var required = await relation.IsRequired();

                if (!required) {
                    continue;
                }

                if (relationValues.TryGetValue(relation.PropertyID, out var values) && values.Any()) {
                    continue;
                }
                else {
                    return false;
                }
            }

            return true;
        }
    }
}