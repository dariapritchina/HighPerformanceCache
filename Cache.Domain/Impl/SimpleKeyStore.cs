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
        CheckKeyIsNotNullOrEmpty(key);
        _keyValues[key] = value;
    }

    public byte[]? Get(string key)
    {
        CheckKeyIsNotNullOrEmpty(key);
        return _keyValues.GetValueOrDefault(key);
    }

    public void Delete(string key)
    {
        if (!_keyValues.ContainsKey(key))
            throw new ArgumentException($"Key \'{key}\' not found");
        _keyValues.Remove(key);
    }
    
    private static void CheckKeyIsNotNullOrEmpty(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key), "Key cannot be null or empty");
    }
}