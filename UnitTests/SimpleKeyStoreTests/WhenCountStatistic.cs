using System.Collections.Concurrent;
using System.Text;
using Cache.Domain.Interfaces;
using UnitTests.DSL;

namespace UnitTests;

public class WhenCountStatistic
{
    [Theory]
    [InlineData(100, 30)]
    public async Task ForMultipleReaders_ShouldCountGetOperations(int readersCount, int operationsPerTask)
    {
        // Arrange
        var storedKey = "anyKey";
        var storedValue = "anyValue"u8.ToArray();
        using var store = Create.Store()
            .WithKeyValue(storedKey, storedValue)
            .Please();
        var random = new Random();
        
        // Act
        var readerTasks = Enumerable.Range(0, readersCount)
            .Select(iReader => Task.Run(async () =>
            {
                for (var i = 0; i < operationsPerTask; i++)
                {
                    store.Get(storedKey);
                    await Task.Delay(random.Next(0, 20));
                }
            }))
            .ToArray();
        await Task.WhenAll(readerTasks);
        var statistics = ((store as IStatisticStore)!).GetStatistic();
        
        // Assert
        Assert.Equal(readersCount * operationsPerTask, statistics.getCount);
    }
    
    [Theory]
    [InlineData(20, 30)]
    public async Task ForMultipleWriters_ShouldCountWriteOperations(int writersCount, int operationsPerTask)
    {
        // Arrange
        var storedKey = "anyKey";
        var storedValue = "anyValue"u8.ToArray();
        using var store = Create.Store().Please();
        var random = new Random();
        var exceptions = new ConcurrentBag<Exception>();
        
        // Act
        var writerTasks = Enumerable.Range(0, writersCount)
            .Select(iWriter => Task.Run(async () =>
            {
                try
                {
                    for (var i = 0; i < operationsPerTask; i++)
                    {
                        store.Set(storedKey, storedValue);
                        await Task.Delay(random.Next(0, 20));
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }))
            .ToArray();
        await Task.WhenAll(writerTasks);
        var statistics = ((store as IStatisticStore)!).GetStatistic();
        
        // Assert
        Assert.Equal(writersCount * operationsPerTask, statistics.setCount);
    }
    
    [Theory]
    [InlineData(20)]
    public async Task ForMultipleDelete_ShouldCountDeleteOperations(int deleteTaskCount)
    {
        // Arrange
        using var store = Create.Store().Please();

        var prepareTasks = Enumerable.Range(0, deleteTaskCount)
            .Select(iTask => Task.Run(async () =>
            {
                var key = $"key_{iTask}";
                var value = Encoding.UTF8.GetBytes($"value_{iTask}");
                store.Set(key, value);
            }))
            .ToArray();
        await Task.WhenAll(prepareTasks);
        
        // Act
        var deleteTasks = Enumerable.Range(0, deleteTaskCount)
            .Select(iTask => Task.Run(async () =>
            {
                var key = $"key_{iTask}";
                store.Delete(key);
            }))
            .ToArray();
        await Task.WhenAll(deleteTasks);
        
        var statistics = ((store as IStatisticStore)!).GetStatistic();
        
        // Assert
        Assert.Equal(deleteTaskCount, statistics.deleteCount);
    }
}