using System;

namespace AuroraCore.Controllers {
    public class EventReactionAttribute : Attribute {
        public int EventID { get; private set; }

        public EventReactionAttribute(int eventID) {
            EventID = eventID;
        }
    }
}