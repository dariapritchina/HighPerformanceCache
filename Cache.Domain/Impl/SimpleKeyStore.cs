using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class SimpleKeyStore : IKeyStore
{
    private readonly Dictionary<string, byte[]> _keyValues = new();
    private readonly ReaderWriterLockSlim _lock = new();
    
    public SimpleKeyStore()
    {
        
    }
    
    public void Set(string key, byte[] value)
    {
        CheckKeyIsNotNullOrEmpty(key);

        try
        {
            _lock.EnterWriteLock();
            
            if (!_keyValues.ContainsKey(key))
                _keyValues.Add(key, value);
            else
                _keyValues[key] = value;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public byte[]? Get(string key)
    {
        CheckKeyIsNotNullOrEmpty(key);
        
        try
        { 
            _lock.EnterReadLock();
            return _keyValues.GetValueOrDefault(key);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Delete(string key)
    {
        try
        {
            _lock.EnterWriteLock();
            
            if (!_keyValues.ContainsKey(key))
                throw new ArgumentException($"Key \'{key}\' not found");
            _keyValues.Remove(key);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
    
    private static void CheckKeyIsNotNullOrEmpty(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key), "Key cannot be null or empty");
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}