using System.Net;
using Cache.Domain.Interfaces;
using Cache.Server;

namespace UnitTests.DSL;

public class TcpServerBuilder
{
    private IKeyStore _store;

    public TcpServerBuilder WithStore(IKeyStore store)
    {
        _store = store;
        return this;
    }
    
    public TcpServer Please()
    {
        var server = new TcpServer(_store);
        
        return server;
    }
}