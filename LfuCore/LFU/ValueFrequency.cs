namespace ConsoleTest.LFU;

public readonly record struct ValueFrequency<TValue>
{
    public required TValue Value { get; init; }
    public required int Frequency { get; init; }
}