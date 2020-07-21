namespace Aurora.Models {
    public class AttrPropertyData {
        public int EventID { get; private set; }
        public int PropertyID { get; private set; }
        public int ValueID { get; private set; }

        public AttrPropertyData(int eventID, int propertyID, int valueID) {
            EventID = eventID;
            PropertyID = propertyID;
            ValueID = valueID;
        }
    }
}