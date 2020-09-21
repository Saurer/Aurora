namespace AuroraCore.Storage {
    public abstract class ConditionRule {
        public class EventConditionRule : ConditionRule {
            public int EventID { get; private set; }

            public EventConditionRule(int eventID) {
                EventID = eventID;
            }
        }
    }
}