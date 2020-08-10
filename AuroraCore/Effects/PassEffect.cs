using System.Threading.Tasks;
using AuroraCore.Storage;

namespace AuroraCore.Effects {
    public class PassEffect : Effect {
        public override async Task Execute(IStorageAdapter _) =>
            await Task.Yield();
    }
}