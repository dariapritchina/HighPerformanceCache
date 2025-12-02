namespace Cache.Domain.Impl;

public readonly ref struct CommandInfo(ReadOnlySpan<char> value, ReadOnlySpan<char> key, ReadOnlySpan<char> command)
{
    public ReadOnlySpan<char> Command { get; } = command;
    public ReadOnlySpan<char> Key { get; } = key;
    public ReadOnlySpan<char> Value { get; } = value;
}