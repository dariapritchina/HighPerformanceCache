using System.Net;
using System.Net.Sockets;

namespace Cache.Client;

public static class SocketFactory
{
    public static Socket Create(IPEndPoint endPoint)
    {
        return new Socket(SocketType.Stream, ProtocolType.Tcp);
    }
}