using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class SimpleKeyStore : IKeyStore, IStatisticStore
{
    private readonly Dictionary<string, byte[]> _keyValues = new();
    private readonly ReaderWriterLockSlim _lock = new();
    
    // statistics
    private long _setCount, _getCount, _deleteCount = 0;
    
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
            
            Interlocked.Increment(ref _setCount);
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
            var value =  _keyValues.GetValueOrDefault(key);
            Interlocked.Increment(ref _getCount);
            return value;
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
            
            Interlocked.Increment(ref _deleteCount);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public (long setCount, long getCount, long deleteCount) GetStatistic()
    {
        return (_setCount, _getCount, _deleteCount);
    }
    
    private static void CheckKeyIsNotNullOrEmpty(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key), "Key cannot be null or empty");
    }

    public void Dispose()
    {
        _lock.Dispose();
        _keyValues.Clear();
    }
}