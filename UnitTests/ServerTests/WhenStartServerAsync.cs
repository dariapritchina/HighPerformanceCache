using System.Net;
using UnitTests.DSL;

namespace UnitTests.ServerTests;

public class WhenStartServerAsync
{
    [Fact]
    public async Task ServerShouldBeNotNull()
    {
        // Arrange
        var server = Create.TcpServer().Please();
        using var ctSource = new CancellationTokenSource();
        ctSource.CancelAfter(500);
        
        // Act
        await server.StartAsync(CreateDefaultEndPoint(), ctSource.Token);
        
        // Assert
        Assert.NotNull(server);

    }
    
    private IPEndPoint CreateDefaultEndPoint()
    {
        var ip = IPAddress.Parse("127.0.0.1");
        var endpoint = new IPEndPoint(ip, 9995);
        
        return endpoint;
    }
}