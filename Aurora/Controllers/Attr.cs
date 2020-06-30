namespace Aurora.Controllers {
    public sealed class Attr {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string DefaultValue { get; private set; }

        public Attr(int id, string name, string defaultValue = null) {
            ID = id;
            Name = name;
            DefaultValue = defaultValue;
        }
    }
}