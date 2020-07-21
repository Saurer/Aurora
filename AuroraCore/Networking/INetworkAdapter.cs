using System;
using System.Net;
using System.Threading.Tasks;

namespace AuroraCore.Networking {
    public interface INetworkAdapter : IDisposable {
        Task Listen();
        Task EmitEvent(EventPacket packet, int position);
        Task Connect(IPEndPoint endPoint);
        event EventHandler<Packet> OnPacket;
    }
}