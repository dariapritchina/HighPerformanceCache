using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class SimpleKeyStore : IKeyStore, IDisposable
{
    private readonly Dictionary<string, byte[]> _keyValues = new();
    private readonly ReaderWriterLockSlim _lock = new();
    
    public SimpleKeyStore()
    {
        
    }
    
    public void Set(string key, byte[] value)
    {
        CheckKeyIsNotNullOrEmpty(key);

        if (!ContainsKey(key))
        {
            DoAddKeyValue(key, value);
        }
        else
        {
            DoUpdateKeyValue(key, value);
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
        if (!ContainsKey(key))
            throw new ArgumentException($"Key \'{key}\' not found");

        try
        {
            _lock.EnterWriteLock(); 
            _keyValues.Remove(key);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private void DoAddKeyValue(string key, byte[] value)
    {
        try
        {
            _lock.EnterWriteLock();
            _keyValues.Add(key, value);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
    
    private void DoUpdateKeyValue(string key, byte[] value)
    {
        try
        {
            _lock.EnterWriteLock();
            _keyValues[key] = value;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private bool ContainsKey(string key)
    {
        try
        {
            _lock.EnterReadLock();
            return _keyValues.ContainsKey(key);
        }
        finally
        {
            _lock.ExitReadLock();;
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