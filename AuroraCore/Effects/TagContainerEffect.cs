using System.Threading.Tasks;
using AuroraCore.Storage;

namespace AuroraCore.Effects {
    public class TagContainerEffect : Effect {
        private readonly int containerID;
        private readonly int providerID;

        public TagContainerEffect(int containerID, int providerID) {
            this.containerID = containerID;
            this.providerID = providerID;
        }

        public override async Task Execute(IStorageAdapter storage) =>
            await storage.AddContainer(containerID, providerID);
    }
}