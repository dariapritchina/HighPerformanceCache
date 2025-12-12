using System.Net;

namespace Cache.Server;

public interface IServer : IDisposable
{
    public Task StartAsync(IPEndPoint endpoint, CancellationToken ct);
}