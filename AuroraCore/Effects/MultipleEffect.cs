using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuroraCore.Storage;

namespace AuroraCore.Effects {
    public class MultipleEffect : Effect {
        private IEnumerable<Effect> effects;

        public MultipleEffect(params Effect[] args) {
            effects = args;
        }

        public override async Task Execute(IStorageAdapter storage) =>
            await Task.WhenAll(effects.Select(l => l.Execute(storage)));
    }
}