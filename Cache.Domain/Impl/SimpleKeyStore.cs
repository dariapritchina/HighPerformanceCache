using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class SimpleKeyStore : IKeyStore
{
    public SimpleKeyStore()
    {
        
    }


    public void Set(string key, byte[] value)
    {
    }

    public byte[]? Get(string key)
    {
        return null;
    }

    public void Delete(string key)
    {
    }
}