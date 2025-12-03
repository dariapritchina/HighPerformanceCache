using UnitTests.DSL;

namespace UnitTests;

public class WhenSetKey
{
    [Fact]
    public void ForEmptyStore_NewKeyWillBeAdded()
    {
        // Arrange
        var store = Create.Store().Please();
        var value = "anyValue"u8.ToArray();
        
        // Act
        store.Set("anyNewKey", value);
        
        // Assert
        Assert.Equal(value, store.Get("anyNewKey"));
    }

    [Fact]
    public void ForExistingKey_ValueWillBeUpdated()
    {
        // Arrange
        const string key = "anyKey";
        var store = Create.Store()
            .WithKeyValue(key, "oldValue"u8.ToArray())
            .Please();
        
        // Act
        store.Set(key, "newValue"u8.ToArray());
        
        // Assert
        Assert.Equal("newValue"u8.ToArray(), store.Get(key));
    }
}