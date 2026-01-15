using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cache.Domain.Impl;
using Cache.Domain.Interfaces;

namespace Cache.Server;

public class TcpServer : IServer
{
    private const string ResponseNull = @"(nil)\r\n";
    private const string ResponseOk = @"OK\r\n";
    private const string ResponseInvalidCommand = @"-ERR Unknown command\r\n";

    
    private readonly IKeyStore _store;
    private Socket? _serverSocket;
    private readonly int _backlog = 100;
    private bool _isDisposed;

    public TcpServer(IKeyStore store)
    {
        _store = store;
    }
    
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
                    var response = ProcessCommand(command);
                    Log($"Response to send: {Encoding.UTF8.GetString(response)}.");
                    await clientSocket.SendAsync(response);
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

    private byte[] ProcessCommand(CommandInfo command)
    {
        byte[] response;
        
        var key = command.Key.ToString();
        switch (command.Command)
        {
            case "GET":
                var value = _store.Get(key);
                response = value ?? Encoding.UTF8.GetBytes(ResponseNull);
                break;
            case "SET":
                var bytesCount = Encoding.UTF8.GetByteCount(command.Value); 
                var byteArray = new byte[bytesCount];
                Encoding.UTF8.GetBytes(command.Value, byteArray);
                _store.Set(key, byteArray);
                response = Encoding.UTF8.GetBytes(ResponseOk);
                break;
            case "DELETE":
                _store.Delete(key);
                response = Encoding.UTF8.GetBytes(ResponseOk);
                break;
            default:
                response = Encoding.UTF8.GetBytes(ResponseInvalidCommand);
                break;
        }

        return response;
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
            _store?.Dispose();
        }

        _isDisposed = true;
    }

    ~TcpServer()
    {
        Dispose(false);
    }
}