using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cache.Client;

do
{
    Console.WriteLine("Enter the message for sending...");
    var message = Console.ReadLine();
    var messageBytes = Encoding.UTF8.GetBytes(message);

    var endPoint = CreateDefaultEndPoint();
    using var clientSocket = SocketFactory.Create(endPoint);

    try
    {
        await clientSocket.ConnectAsync(endPoint, CancellationToken.None);
        var bytesSent = await clientSocket.SendAsync(messageBytes, SocketFlags.None);
        Console.WriteLine($"Bytes sent: {bytesSent}.");

        var receivedMessage = await ReadAnswer(clientSocket);
        Console.WriteLine(receivedMessage);

        clientSocket.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    finally
    {
        if (clientSocket.Connected)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
        }

        clientSocket.Close();
    }
}
while (NeedOneMoreMessage());

bool NeedOneMoreMessage()
{
    Console.WriteLine("One more message? (Y/N)");
    var answer = Console.ReadLine();
    return (answer != "N");
}

IPEndPoint CreateDefaultEndPoint()
{
    var ip = IPAddress.Parse("127.0.0.1");
    var endpoint = new IPEndPoint(ip, 9995);
        
    return endpoint;
}

async Task<string> ReadAnswer(Socket clientSocket)
{
    var arrayPool = ArrayPool<byte>.Shared;
    var memoryBuffer = arrayPool.Rent(1024);
    var bytesReceived = await clientSocket.ReceiveAsync(memoryBuffer, SocketFlags.None);
    var receivedMessage = Encoding.UTF8.GetString(memoryBuffer, 0, bytesReceived);

    return receivedMessage;
}