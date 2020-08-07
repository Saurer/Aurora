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

        public async Task<IEnumerable<IAttachedProperty<IAttr>>> GetAttributes() =>
            await TraverseParentToCollection(model =>
                context.Storage.GetPropertyProviderAttributes(model.ModelID)
            );

        public async Task<IEnumerable<IAttachedProperty<IRelation>>> GetRelations() =>
            await TraverseParentToCollection(model =>
                context.Storage.GetPropertyProviderRelations(model.ModelID)
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