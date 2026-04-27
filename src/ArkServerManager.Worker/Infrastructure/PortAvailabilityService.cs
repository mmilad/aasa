using System.Net;
using System.Net.Sockets;

namespace ArkServerManager.Worker.Infrastructure;

public interface IPortAvailabilityService
{
    bool IsAvailable(int port);
}

public sealed class PortAvailabilityService : IPortAvailabilityService
{
    public bool IsAvailable(int port)
    {
        try
        {
            using var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }
}
