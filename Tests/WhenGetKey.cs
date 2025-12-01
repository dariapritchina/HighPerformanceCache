using Tests.DSL;

namespace Tests;

public class WhenGetKey
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ForNotExistingKey_ReturnsNull()
    {
        // Arrange
        var keyStore = Create.Store().Please();
        
        // Act
        var value = keyStore.Get("somethingKey");
        
        // Assert
        Assert.That(value, Is.Null);
    }
    
    [Test]
    public void ForExistingKey_ReturnsValue()
    {
        // Arrange
        var keyStore = Create.Store()
            .WithKeyValue("somethingKey", "anyValue"u8.ToArray())
            .Please();
        
        // Act
        var value = keyStore.Get("somethingKey");
        
        // Assert
        Assert.That(value, Is.EqualTo("anyValue"u8.ToArray()));
    }
}