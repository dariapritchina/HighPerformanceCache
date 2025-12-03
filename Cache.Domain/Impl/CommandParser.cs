using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class CommandParser : ICommandParser
{
    private const char SEPARATOR = ' ';
    private const int NOT_FOUND_INDEX = -1;
    public static CommandInfo Parse(ReadOnlySpan<char> input)
    {
        var command = SliceNextPart(input);
        var key = SliceNextPart(input, command.Length + 1);
        var value = SliceNextPart(input, command.Length + 1 + key.Length + 1);

        return new CommandInfo(
            command: command,
            key: key,
            value: value);
    }

    private static ReadOnlySpan<char> SliceNextPart(ReadOnlySpan<char> input, int startFrom = 0)
    {
        if (startFrom >= input.Length)
        {
            return default;
        }
        
        input = input[startFrom..];
        var firstIndexOfSeparator = input.IndexOf(SEPARATOR);

        return firstIndexOfSeparator == NOT_FOUND_INDEX 
            ? input 
            : input[..firstIndexOfSeparator];
    }
}