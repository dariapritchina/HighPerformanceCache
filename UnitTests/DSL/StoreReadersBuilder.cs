using Cache.Domain.Impl;
using Cache.Domain.Interfaces;

namespace UnitTests.DSL;

public class StoreReadersBuilder
{
    private int _readersCount;
    private IKeyStore _store;
    private string _key;
    private int _operationsPerTask;
    public StoreReadersBuilder Count(int readersCount)
    {
        _readersCount = readersCount;
        return this;
    }

    public StoreReadersBuilder FromStore(IKeyStore store, string key)
    {
        _store = store;
        _key = key;
        return this;
    }

    public StoreReadersBuilder Times(int operationsPerTask)
    {
        _operationsPerTask = operationsPerTask;
        return this;
    }

    public IEnumerable<Task> Please()
    {
        var random = new Random();
        
        return Enumerable.Range(0, _readersCount)
            .Select(iReader => Task.Run(async () =>
            {
                for (var iOp = 0; iOp < _operationsPerTask; iOp++)
                {
                    _store.Get(_key);
                    await Task.Delay(random.Next(0, 20));
                }
            }))
            .ToArray();
    }
}