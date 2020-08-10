using System.Threading.Tasks;
using AuroraCore.Storage;

namespace AuroraCore.Effects {
    public abstract class Effect {
        public static Effect Pass =>
            new PassEffect();

        public abstract Task Execute(IStorageAdapter storage);
    }
}