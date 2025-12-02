using Cache.Domain.Impl;

namespace Cache.Domain.Interfaces;

public interface ICommandParser
{
    public static abstract CommandInfo Parse(ReadOnlySpan<char> command);
}