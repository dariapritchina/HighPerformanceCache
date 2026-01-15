using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cache.Client.Interfaces;

namespace Cache.Client.Impl;

public class SimpleTcpClient : ITcpClient
{
    private readonly IPEndPoint _endPoint;
    private Socket? _clientSocket;

    public SimpleTcpClient(IPEndPoint endPoint)
    {
        _endPoint = endPoint;
    }
    
    public async Task ConnectAsync()
    {
        _clientSocket = SocketFactory.Create(_endPoint);

        try
        {
            await _clientSocket.ConnectAsync(_endPoint, CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<string> SetAsync(string key, byte[] value)
    {
        CheckClientSocket();

        var receivedMessage = string.Empty;
        
        if (_clientSocket != null)
        {
            var messageBytes = ToSetCommandMessage(key, value);
            var bytesSent = await _clientSocket.SendAsync(messageBytes, SocketFlags.None);
            receivedMessage = await ReadAnswer(_clientSocket);
            Console.WriteLine(receivedMessage);
        }
        
        return receivedMessage;
    }

    public async Task<string> GetAsync(string key)
    {
        CheckClientSocket();
        
        var receivedMessage = string.Empty;

        if (_clientSocket != null)
        {
            var messageBytes = ToGetCommandMessage(key);
            var bytesSent = await _clientSocket.SendAsync(messageBytes, SocketFlags.None);
            receivedMessage = await ReadAnswer(_clientSocket);
            Console.WriteLine(receivedMessage);
        }
        
        return receivedMessage;
    }
    
    private void CheckClientSocket()
    {
        if (_clientSocket == null)
            throw new ArgumentNullException($"Client socket is null.");
    }
    
    async Task<string> ReadAnswer(Socket clientSocket)
    {
        var arrayPool = ArrayPool<byte>.Shared;
        var memoryBuffer = arrayPool.Rent(1024);
        var bytesReceived = await clientSocket.ReceiveAsync(memoryBuffer, SocketFlags.None);
        var receivedMessage = Encoding.UTF8.GetString(memoryBuffer, 0, bytesReceived);

        return receivedMessage;
    }

    private byte[] ToSetCommandMessage(string key, byte[] value)
    {
        var message = $"SET {key} {Encoding.UTF8.GetString(value)}";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        return messageBytes;
    }
    
    private byte[] ToGetCommandMessage(string key)
    {
        var message = $"GET {key}";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        return messageBytes;
    }

    public void Dispose()
    {
        if (_clientSocket == null) return;
        
        if (_clientSocket.Connected)
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
        }

        _clientSocket.Close();
        _clientSocket?.Dispose();
    }
}