using System.Collections.Concurrent;
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
}