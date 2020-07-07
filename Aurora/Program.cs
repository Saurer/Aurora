using System;
using System.Threading.Tasks;
using AuroraCore.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Aurora {
    public class EventData : IEvent {
        public int ID { get; set; }
        public int BaseEventID { get; set; }
        public int ValueID { get; set; }
        public int ConditionEventID { get; set; }
        public int ActorEventID { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }
    }

    class Program {
        public static async Task Main(string[] args) {
            foreach (var e in Tables.Table) {
                await Engine.Instance.ProcessEvent(e);
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
