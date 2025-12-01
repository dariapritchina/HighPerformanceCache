using Cache.Domain.Impl;
using Cache.Domain.Interfaces;

namespace Tests.DSL;

public class KeyStoreBuilder
{
    public IKeyStore Please()
    {
        return new SimpleKeyStore();
    }
}