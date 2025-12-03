using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class CommandParser : ICommandParser
{
    private const char SEPARATOR = ' ';
    public static CommandInfo Parse(ReadOnlySpan<char> input)
    {
        var command = SliceNextPart(input);
        var key = SliceNextPart(input, command.Length + 1);
        var value = input.Slice(command.Length + 1 + key.Length + 1);

        return new CommandInfo(
            command: command,
            key: key,
            value: value);
    }

    private static ReadOnlySpan<char> SliceNextPart(ReadOnlySpan<char> input, int startFrom = 0)
    {
        input = input.Slice(startFrom);
        var firstIndexOfSeparator = input.IndexOf(SEPARATOR);

        var part = input.Slice(0, firstIndexOfSeparator);
        return part;
    }
}