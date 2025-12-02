using Tests.DSL;

namespace Tests;

public class WhenDeleteKey
{
    [Test]
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
        Assert.That(store.Get(anyKey), Is.Null);
    }
}