using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class SimpleKeyStore : IKeyStore
{
    private readonly Dictionary<string, byte[]> _keyValues = new();
    
    public SimpleKeyStore()
    {
        
    }
    
    public void Set(string key, byte[] value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key), "Key cannot be null or empty");
        
        _keyValues[key] = value;
    }

    public byte[]? Get(string key)
    {
        return _keyValues.GetValueOrDefault(key);
    }

    public void Delete(string key)
    {
        _keyValues.Remove(key);
    }
}