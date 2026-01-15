namespace Cache.Client.Interfaces;

public interface ITcpClient : IDisposable
{
    public Task ConnectAsync();
    public Task<string> SetAsync(string key, byte[] value);
    public Task<string> GetAsync(string key);
}