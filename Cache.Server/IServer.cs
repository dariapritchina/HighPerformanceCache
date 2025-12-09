using System.Net;

namespace Cache.Server;

public interface IServer
{
    public Task StartAsync(IPEndPoint endpoint, CancellationToken ct);
}