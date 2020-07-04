namespace Aurora.Models {
    public class AttrPropertyData {
        public int ID { get; private set; }
        public int ValueID { get; private set; }

        public AttrPropertyData(int id, int valueID) {
            ID = id;
            ValueID = valueID;
        }
    }
}