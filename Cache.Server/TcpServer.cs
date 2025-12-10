using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cache.Server;

public class TcpServer : IServer
{
    private readonly int _backlog = 100;
    
    public async Task StartAsync(IPEndPoint endpoint, CancellationToken ct)
    {
        try
        {
            using var serverSocket = CreateServerSocket(endpoint);
            serverSocket.Listen(_backlog);

            while (true)
            {
                var clientSocket = await serverSocket.AcceptAsync(ct);
                await ProcessClientAsync(clientSocket, ct);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task ProcessClientAsync(Socket clientSocket, CancellationToken ct)
    {
        var buffer = new byte[1024];
        var arrayPool = ArrayPool<byte>.Shared;
        var memoryBuffer = arrayPool.Rent(1024);
        
        try
        {
            while (true)
            {
                var bytesReceived = await clientSocket.ReceiveAsync(memoryBuffer, SocketFlags.None);

                if (bytesReceived == 0)
                {
                    Console.WriteLine("Connection closed by remote host.");
                    break;
                }

                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine($"Received: {receivedMessage}"); 
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
        }
        finally
        {
            arrayPool.Return(memoryBuffer);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    private Socket CreateServerSocket(IPEndPoint endpoint)
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endpoint);
        
        return socket;
    }
}