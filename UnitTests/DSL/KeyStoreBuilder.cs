using Cache.Domain.Impl;
using Cache.Domain.Interfaces;

namespace UnitTests.DSL;

public class KeyStoreBuilder
{
    private string? _key;
    private byte[]? _value;
    
    public IKeyStore Please()
    {
        var store = new SimpleKeyStore();

        if ((_key != null) && (_value != null))
        {
            store.Set(_key, _value);
        }
        
        return store;
    }

    public KeyStoreBuilder WithKeyValue(string key, byte[]? value)
    {
        _key = key;
        _value = value;
        
        return this;
    }
}