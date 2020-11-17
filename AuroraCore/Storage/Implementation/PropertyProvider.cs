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
    internal class PropertyProvider : IPropertyProvider {
        private readonly int providerID;
        private readonly IDataContext context;

        public PropertyProvider(IDataContext context, int providerID) {
            this.context = context;
            this.providerID = providerID;
        }

        public async Task<IAttachedProperty<IAttr>> GetAttribute(int attributeID) =>
            await context.Storage.GetPropertyProviderAttribute(providerID, attributeID);

        public async Task<IAttachedProperty<IRelation>> GetRelation(int relationID) =>
            await context.Storage.GetPropertyProviderRelation(providerID, relationID);

        public async Task<IEnumerable<IAttachedProperty<IAttr>>> GetAttributes() =>
            await context.Storage.GetPropertyProviderAttributes(providerID);

        public async Task<IEnumerable<IAttachedProperty<IRelation>>> GetRelations() =>
            await context.Storage.GetPropertyProviderRelations(providerID);
    }
}