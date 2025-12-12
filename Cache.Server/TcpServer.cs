using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cache.Domain.Impl;

namespace Cache.Server;

public class TcpServer : IServer
{
    private Socket? _serverSocket;
    private readonly int _backlog = 100;
    private bool _isDisposed;
    
    public async Task StartAsync(IPEndPoint endpoint, CancellationToken ct)
    {
        try
        {
            _serverSocket = CreateServerSocket(endpoint);
            Log("Server socket created.");
            _serverSocket.Listen(_backlog);

            while (!ct.IsCancellationRequested)
            {
                var clientSocket = await _serverSocket.AcceptAsync(ct);
                await Task.Run(() => ProcessClientAsync(clientSocket, ct), ct)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            Log($"Error when accepting client {t.Exception.Message}");
                        }
                    }, ct);
            }
        }
        catch (OperationCanceledException)
        {
            Log("Operation was cancelled.");
        }
        catch (Exception e)
        {
            Log(e.ToString());
            throw;
        }
    }

    private async Task ProcessClientAsync(Socket clientSocket, CancellationToken ct)
    {
        var arrayPool = ArrayPool<byte>.Shared;
        var memoryBuffer = arrayPool.Rent(1024);
        
        try
        {
            while (true)
            {
                var bytesReceived = await clientSocket.ReceiveAsync(memoryBuffer, SocketFlags.None);

                if (bytesReceived == 0)
                {
                    Log("Connection closed by remote host.");
                    break;
                }

                var receivedMessage = Encoding.UTF8.GetString(memoryBuffer, 0, bytesReceived);
                try
                {
                    var command = CommandParser.Parse(receivedMessage);
                    Log($"Received command: command=\'{command.Command}\', key=\'{command.Key}\', value=\'{command.Value}\'."); 
                }
                catch (Exception e)
                {
                    Log($"Error when parse command {receivedMessage}: {e.Message}");
                    throw;
                }
            }
        }
        catch (SocketException ex)
        {
            Log($"Socket Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Log($"General Error: {ex.Message}");
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

    private void Log(string message)
    {
        Console.WriteLine(message);
    }

    public void Dispose()
    {
        Dispose(true);
        
        // Предотвращаем попадание объекта в Finalization queue
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool isManual)
    {
        if (_isDisposed) return;

        if (isManual)
        {
            _serverSocket?.Dispose();
        }

        _isDisposed = true;
    }

    ~TcpServer()
    {
        Dispose(false);
    }
}