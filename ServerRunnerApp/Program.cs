using System.Net;
using Cache.Domain.Impl;
using Cache.Server;

var ip = IPAddress.Parse("127.0.0.1");
var endpoint = new IPEndPoint(ip, 9995);
var store = new SimpleKeyStore();
using var server = new TcpServer(store);

Console.WriteLine("Server is starting...");

try
{
    await server.StartAsync(endpoint, CancellationToken.None);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}