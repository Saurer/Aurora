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