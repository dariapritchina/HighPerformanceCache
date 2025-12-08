using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class CommandParser : ICommandParser
{
    private const char SEPARATOR = ' ';
    private const int NOT_FOUND_INDEX = -1;
    public static CommandInfo Parse(ReadOnlySpan<char> input)
    {
        input = input.Trim();
        
        var command = ReadNextPart(input);
        if (command.IsEmpty)
            throw new ArgumentException("Command is empty.");
        input = CutPart(input, command);
        
        var key = ReadNextPart(input);
        if (key.IsEmpty)
            return default;
        input = CutPart(input, key);
        
        return new CommandInfo(
            command: command,
            key: key,
            value: input.IsEmpty ? default : input);
    }

    private static ReadOnlySpan<char> CutPart(ReadOnlySpan<char> input, ReadOnlySpan<char> part)
    {
        if (input.Length > part.Length)
        {
            input = input[(part.Length + 1)..].TrimStart();
        }
        else if (input.Length == part.Length)
        {
            // Cut all the chars
            input = ReadOnlySpan<char>.Empty;
        }

        return input;
    }

    private static ReadOnlySpan<char> ReadNextPart(ReadOnlySpan<char> input)
    {
        var indexOfSeparator = input.IndexOf(SEPARATOR);
        var part = (indexOfSeparator == NOT_FOUND_INDEX)
            ? input
            : input[..indexOfSeparator];

        return part;
    }
}