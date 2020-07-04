using System;
using Aurora.Controllers;
using Aurora.DataTypes;
using AuroraCore.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
        public static void Main(string[] args) {
            Engine.Instance.AddController<EventController>();
            Engine.Instance.AddType<BasicType>("basic_type");

            foreach (var e in Tables.Table) {
                Engine.Instance.ProcessEvent(e);
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
