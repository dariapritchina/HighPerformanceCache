using System.Collections.Concurrent;
using System.Text;
using Cache.Domain.Interfaces;
using UnitTests.DSL;

namespace UnitTests;

public class WhenUseConcurrentAccess
{
    [Theory]
    [InlineData(100, 30)]
    public async Task WithMultipleReaders_ShouldHasNoExceptions(int readersCount, int operationsPerTask)
    {
        // Arrange
        var storedKey = "anyKey";
        var storedValue = "anyValue"u8.ToArray();
        using var store = Create.Store()
            .WithKeyValue(storedKey, storedValue)
            .Please();
        var random = new Random();
        var exceptions = new ConcurrentBag<Exception>();
        
        // Act
        var readerTasks = Enumerable.Range(0, readersCount)
            .Select(iReader => Task.Run(async () =>
            {
                try
                {
                    for (var i = 0; i < operationsPerTask; i++)
                    {
                        var value = store.Get(storedKey);
                        await Task.Delay(random.Next(0, 20));
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }))
            .ToArray();
        await Task.WhenAll(readerTasks);
        
        // Assert
        Assert.Empty(exceptions);
    }
    
    [Theory]
    [InlineData(100, 30)]
    public async Task WithMultipleReaders_ShouldReadCorrectValue(int readersCount, int operationsPerTask)
    {
        // Arrange
        var storedKey = "anyKey";
        var storedValue = "anyValue"u8.ToArray();
        using var store = Create.Store()
            .WithKeyValue(storedKey, storedValue)
            .Please();
        var random = new Random();
        var readValues = new ConcurrentBag<byte[]?>();
        
        // Act
        var readerTasks = Enumerable.Range(0, readersCount)
            .Select(iReader => Task.Run(async () =>
            {
                for (var i = 0; i < operationsPerTask; i++)
                {
                    var value = store.Get(storedKey);
                    readValues.Add(value);
                    await Task.Delay(random.Next(0, 20));
                }
            }))
            .ToArray();
        await Task.WhenAll(readerTasks);
        
        // Assert
        Assert.All(readValues.ToArray(), v => Assert.Equal(v, storedValue));
    }
    
    [Theory]
    [InlineData(100, 30)]
    public async Task WithMultipleReaders_StatisticsShouldCountGetOperations(int readersCount, int operationsPerTask)
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
    [InlineData(1000, 30)]
    public async Task WithMultipleWriters_ShouldHasNoExceptions(int writersCount, int operationsPerTask)
    {
        // Arrange
        var storedKey = "anyKey";
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
                        store.Set(storedKey, Encoding.UTF8.GetBytes($"any_value_{i}"));
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
        
        // Assert
        Assert.Empty(exceptions);
    }
    
    [Theory]
    [InlineData(100, 100, 30)]
    public async Task WithMultipleReadersAndWriters_ShouldHasNoExceptions(int readersCount, int writersCount, int operationsPerTask)
    {
        // Arrange
        var storedKey = "anyKey";
        var storedValue = "anyValue"u8.ToArray();
        using var store = Create.Store()
            .WithKeyValue(storedKey, storedValue)
            .Please();
        var random = new Random();
        var exceptions = new ConcurrentBag<Exception>();
        
        // Act
        var readerTasks = Enumerable.Range(0, readersCount)
            .Select(iReader => Task.Run(async () =>
            {
                try
                {
                    for (var i = 0; i < operationsPerTask; i++)
                    {
                        var value = store.Get(storedKey);
                        await Task.Delay(random.Next(0, 20));
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }))
            .ToArray();
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
        await Task.WhenAll(readerTasks.Concat(writerTasks));
        
        // Assert
        Assert.Empty(exceptions);
    }
}