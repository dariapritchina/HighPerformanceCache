namespace Cache.Domain.Interfaces;

public interface IKeyStore : IDisposable
{
    void Set(string key, byte[] value);
    byte[]? Get(string key);
    void Delete(string key);
}