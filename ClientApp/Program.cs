using System.Net;
using System.Net.Sockets;
using System.Text;

do
{
    Console.WriteLine("Enter the message for sending...");
    var message = Console.ReadLine();
    var messageBytes = Encoding.UTF8.GetBytes(message);

    var endPoint = CreateDefaultEndPoint();
    using var clientSocket = CreateClientSocket(endPoint);

    try
    {
        await clientSocket.ConnectAsync(endPoint, CancellationToken.None);
        var bytesSent = await clientSocket.SendAsync(messageBytes, SocketFlags.None);
        Console.WriteLine($"Bytes sent: {bytesSent}.");

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
while (!NeedOneMoreMessage());

bool NeedOneMoreMessage()
{
    Console.WriteLine("One more message? (Y/N)");
    var answer = Console.ReadLine();
    return (answer != "N");
}

Socket CreateClientSocket(IPEndPoint endPoint)
{
    return new Socket(SocketType.Stream, ProtocolType.Tcp);
}

IPEndPoint CreateDefaultEndPoint()
{
    var ip = IPAddress.Parse("127.0.0.1");
    var endpoint = new IPEndPoint(ip, 9995);
        
    return endpoint;
}