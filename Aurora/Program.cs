using System;
using Aurora.Controllers;
using Aurora.DataTypes;
using AuroraCore;
using AuroraCore.Storage;

namespace Aurora {
    public class EventData : IEventData {
        public int ID { get; set; }
        public int BaseEventID { get; set; }
        public int ValueID { get; set; }
        public int ConditionEventID { get; set; }
        public int ActorEventID { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }
    }

    class Program {
        static void Main(string[] args) {
            var engine = new EngineBase();
            engine.AddController<EventController>();
            engine.AddType<BasicType>("basic_type");

            foreach (var e in Tables.Table) {
                engine.ProcessEvent(e);
            }

            Console.WriteLine("Done");
        }
    }
}
