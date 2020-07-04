namespace AuroraCore.Events {
    public abstract class Event {
        public int ID { get; private set; }

        public Event(int id) {
            ID = id;
        }
    }
}