namespace AuroraCore.Storage {
    public interface IRelation : IIndividual {
    }

    internal sealed class Relation : Individual, IRelation {
        public Relation(IDataContext context, IEvent e) : base(context, e) {

        }
    }
}