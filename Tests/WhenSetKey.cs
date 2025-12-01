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
}