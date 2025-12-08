using UnitTests.DSL;

namespace UnitTests;

public class WhenDeleteKey
{
    [Fact]
    public void KeyAndValueWillBeDeleted()
    {
        // Arrange
        const string anyKey = "anyKey"; 
        var store = Create.Store()
            .WithKeyValue(anyKey, "anyValue"u8.ToArray())
            .Please();
        
        // Act
        store.Delete(anyKey);
        
        // Assert
        Assert.Null(store.Get(anyKey));
    }

    [Fact]
    public void ForNotExistingKey_ShouldThrowAnException()
    {
        // Arrange
        var store = Create.Store().Please();
        
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => store.Delete("anyNotExisingKey"));
    }
}