using Cache.Domain.Impl;

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
        var keyStore = new SimpleKeyStore();
        
        // Act
        var key = keyStore.Get("somethingKey");
        
        // Assert
        Assert.That(key, Is.Null);
    }
}