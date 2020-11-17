// Aurora 
// Copyright (C) 2020  Frank Horrigan <https://github.com/saurer>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Net;
using System.Threading.Tasks;
using Aurora.Configuration;
using Aurora.Networking.Tcp;
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

        public EventData(int id, int baseEventID, int valueID, int conditionEventID, int actorEventID, string value) {
            ID = id;
            BaseEventID = baseEventID;
            ValueID = valueID;
            ConditionEventID = conditionEventID;
            ActorEventID = actorEventID;
            Value = value;
            Date = DateTime.UtcNow;
        }
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
