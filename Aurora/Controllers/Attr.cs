namespace Aurora.Controllers {
    public sealed class Attr {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public Attr(int id, string name) {
            ID = id;
            Name = name;
        }
    }
}