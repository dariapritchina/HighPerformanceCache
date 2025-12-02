using Cache.Domain.Interfaces;

namespace Cache.Domain.Impl;

public class CommandParser : ICommandParser
{
    public static CommandInfo Parse(ReadOnlySpan<char> command)
    {
        throw new NotImplementedException();
    }
}