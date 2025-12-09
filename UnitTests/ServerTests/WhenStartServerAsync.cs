using System.Net;
using System.Net.Sockets;
using Cache.Server;
using Moq;
using UnitTests.DSL;

namespace UnitTests.ServerTests;

public class WhenStartServerAsync
{
    [Fact]
    public async Task ServerShouldBeNotNull()
    {
        // Arrange
        var server = Create.TcpServer().Please();
        
        // Act
        await server.StartAsync(CreateDefaultEndPoint(), CancellationToken.None);
        
        // Assert
        Assert.NotNull(server);

    }

    private IPEndPoint CreateDefaultEndPoint()
    {
        var ip = IPAddress.Parse("127.0.0.1");
        var endpoint = new IPEndPoint(ip, 8080);
        
        return endpoint;
    }
}