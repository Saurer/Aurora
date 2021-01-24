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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal class ModelPropertyProvider : IPropertyProvider {
        private readonly IModel model;
        private readonly IDataContext context;

        public ModelPropertyProvider(IDataContext context, IModel model) {
            this.context = context;
            this.model = model;
        }

        public async Task<IAttachedProperty<IAttr>> GetAttribute(int attributeID) =>
            await TraverseParentToEncounter(model =>
                context.Storage.GetPropertyProviderAttribute(model.ModelID, attributeID)
            );

        public async Task<IAttachedProperty<IRelation>> GetRelation(int relationID) =>
            await TraverseParentToEncounter(model =>
                context.Storage.GetPropertyProviderRelation(model.ModelID, relationID)
            );

        public async Task<IAttachedEvent<IProperty>> GetEvent(int propertyID) =>
            await TraverseParentToEncounter(model =>
                context.Storage.GetPropertyProviderEvent(model.ModelID, propertyID)
            );

        public async Task<IEnumerable<IAttachedProperty<IAttr>>> GetAttributes() =>
            await TraverseParentToCollection(model =>
                context.Storage.GetPropertyProviderAttributes(model.ModelID)
            );

        public async Task<IEnumerable<IAttachedProperty<IRelation>>> GetRelations() =>
            await TraverseParentToCollection(model =>
                context.Storage.GetPropertyProviderRelations(model.ModelID)
            );

        public async Task<IEnumerable<IAttachedEvent<IProperty>>> GetEvents() =>
            await TraverseParentToCollection(model =>
                context.Storage.GetPropertyProviderEvents(model.ModelID)
            );

        private async Task<IEnumerable<T>> TraverseParentToCollection<T>(Func<IModel, Task<IEnumerable<T>>> getter) {
            var queue = new Queue<IModel>(new[] { model });
            var result = new List<T>();

            while (queue.Count > 0) {
                var model = queue.Dequeue();
                var values = await getter.Invoke(model);
                result.AddRange(values);

                var parent = await model.GetParentModel();
                if (null != parent) {
                    queue.Enqueue(parent);
                }
            }

            return result;
        }

        private async Task<T> TraverseParentToEncounter<T>(Func<IModel, Task<T>> getter) {
            var queue = new Queue<IModel>(new[] { model });
            var result = new List<T>();

            while (queue.Count > 0) {
                var model = queue.Dequeue();
                var value = await getter.Invoke(model);

                if (null != value) {
                    return value;
                }

                var parent = await model.GetParentModel();
                if (null != parent) {
                    queue.Enqueue(parent);
                }
            }

            return default(T);
        }
    }
}