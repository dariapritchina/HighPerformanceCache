using Cache.Domain.Impl;

namespace UnitTests;

public class WhenParseCommand
{
    [Theory]
    [InlineData("SET user:1 anyData", "SET", "user:1", "anyData")]
    public void WithThreeArgs_ReturnsCommandInfoWith_Command_Key_Value(
        string input,
        string expectedCommand, string expectedKey, string expectedValue)
    {
        // Arrange
        var spanInput = input.AsSpan();
        
        // Act
        var parsedCommand = CommandParser.Parse(input);
        
        // Assert
        Assert.Equal(expectedCommand, parsedCommand.Command);
        Assert.Equal(expectedKey, parsedCommand.Key);
        Assert.Equal(expectedValue, parsedCommand.Value);
    }

    [Theory]
    [InlineData("GET user:1", "GET", "user:1")]
    public void WithTwoArgs_ReturnsCommandInfoWith_CommandAndKey(
        string input,
        string expectedCommand, string expectedKey)
    {
        // Arrange
        var spanInput = input.AsSpan();
        
        // Act
        var parsedCommand = CommandParser.Parse(input);
        
        // Assert
        Assert.Equal(expectedCommand, parsedCommand.Command);
        Assert.Equal(expectedKey, parsedCommand.Key);
        Assert.True(parsedCommand.Value.IsEmpty);
    }

    [Theory]
    [InlineData("GET")]
    public void WithoutKeyPart_ShouldThrowAnException(string input)
    {
        // Arrange
        var spanInput = input.AsSpan();
        
       // Assert
        Assert.Throws<ArgumentException>(() => CommandParser.Parse(input));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void WithOnlySpaces_ShouldThrowAnException(string input)
    {
        // Arrange
        var spanInput = input.AsSpan();
        
        // Assert
        Assert.Throws<ArgumentException>(() => CommandParser.Parse(input));
    }
    
    [Theory]
    [InlineData(" SET user:1 anyData", "SET", "user:1", "anyData")]
    [InlineData("  SET user:1 anyData", "SET", "user:1", "anyData")]
    [InlineData("SET     user:1 anyData", "SET", "user:1", "anyData")]
    [InlineData("SET user:1    anyData", "SET", "user:1", "anyData")]
    [InlineData("SET user:1 anyData    ", "SET", "user:1", "anyData")]
    [InlineData("   SET     user:1      anyData   ", "SET", "user:1", "anyData")]
    public void WithManySpacesBetweenTreeArgs_ShouldParseCorrect(string input,
        string expectedCommand, string expectedKey, string expectedValue)
    {
        // Arrange
        var spanInput = input.AsSpan();
        
        // Act
        var parsedCommand = CommandParser.Parse(input);
        
        // Assert
        Assert.Equal(expectedCommand, parsedCommand.Command);
        Assert.Equal(expectedKey, parsedCommand.Key);
        Assert.Equal(expectedValue, parsedCommand.Value);
    }
    
    [Theory]
    [InlineData(" GET user:1", "GET", "user:1")]
    [InlineData("    GET user:1", "GET", "user:1")]
    [InlineData(" GET     user:1", "GET", "user:1")]
    [InlineData("  GET     user:1    ", "GET", "user:1")]
    [InlineData("GET user:1    ", "GET", "user:1")]
    public void WithManySpacesBetweenTwoArgs_ShouldParseCorrect(
        string input,
        string expectedCommand, string expectedKey)
    {
        // Arrange
        var spanInput = input.AsSpan();
        
        // Act
        var parsedCommand = CommandParser.Parse(input);
        
        // Assert
        Assert.Equal(expectedCommand, parsedCommand.Command);
        Assert.Equal(expectedKey, parsedCommand.Key);
        Assert.True(parsedCommand.Value.IsEmpty);
    }
}