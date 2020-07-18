using System;
using System.Net;
using System.Threading.Tasks;
using Aurora.Configuration;
using Aurora.Networking.Tcp;
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
            Engine.Instance.AddNetworkAdapter(new TcpNetworkAdapter(new IPEndPoint(IPAddress.Any, 20854)));
            var config = AuroraConfig.Load("./config.yaml");

            foreach (var network in config.Network) {
                var endpoint = IPEndPoint.Parse(network.Endpoint);
                await Engine.Instance.Connect<TcpNetworkAdapter>(endpoint);
            }

            foreach (var e in Tables.Table) {
                await Engine.Instance.ProcessEvent(e);
            }

            // CreateHostBuilder(args).Build().Run();
            Console.Read();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
