using UnitTests.DSL;

namespace UnitTests;

public class WhenGetKey
{
    [Fact]
    public void ForNotExistingKey_ReturnsNull()
    {
        // Arrange
        using var keyStore = Create.Store().Please();
        
        // Act
        var value = keyStore.Get("somethingKey");
        
        // Assert
        Assert.Null(value);
    }
    
    [Fact]
    public void ForExistingKey_ReturnsValue()
    {
        // Arrange
        using var keyStore = Create.Store()
            .WithKeyValue("somethingKey", "anyValue"u8.ToArray())
            .Please();
        
        // Act
        var value = keyStore.Get("somethingKey");
        
        // Assert
        Assert.Equal("anyValue"u8.ToArray(), value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void ForNullOrEmptyOrWhitespaceKey_ShouldThrowAnException(string? key)
    {
        // Arrange
        using var keyStore = Create.Store().Please();
        
        // Assert
        Assert.Throws<ArgumentNullException>(() => keyStore.Get(key));
    }
}