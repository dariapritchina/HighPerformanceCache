using System.Net;
using System.Net.Sockets;

namespace Cache.Server;

public class TcpServer : IServer
{
    private readonly int _backlog = 100;
    
    public async Task StartAsync(IPEndPoint endpoint, CancellationToken ct)
    {
        try
        {
            using var socket = CreateSocket(endpoint);
            socket.Listen(_backlog);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private Socket CreateSocket(IPEndPoint endpoint)
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endpoint);
        
        return socket;
    }
}