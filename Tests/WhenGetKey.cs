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
        var key = keyStore.Get("somethingKey");
        
        // Assert
        Assert.That(key, Is.Null);
    }
}