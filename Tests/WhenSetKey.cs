using System.Text;
using Tests.DSL;

namespace Tests;

public class WhenSetKey
{
    [Test]
    public void ForEmptyStore_NewKeyWillBeAdded()
    {
        // Arrange
        var store = Create.Store().Please();
        var value = "anyValue"u8.ToArray();
        
        // Act
        store.Set("anyNewKey", value);
        
        // Assert
        Assert.That(store.Get("anyNewKey"), Is.EqualTo(value));
    }

    [Test]
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
        Assert.That(store.Get(key), Is.EqualTo("newValue"u8.ToArray()));
    }
}