namespace ConsoleTest.LFU;

/// <summary>
/// Lfu items with same frequency form linked list
/// </summary>
/// <typeparam name="TValue">type of value in item</typeparam>
/// <typeparam name="TKey">type of key in item</typeparam>
public readonly struct LfuItem<TValue,TKey>
{
    public required int Frequency { get; init; }
    public required TValue Value { get; init; }
    public required TKey Key { get; init; }
    
    public required Node<Frequency<TValue,TKey>> FrequencyNode { get; init; }
    
    public LfuItem<TValue, TKey> UpgradeFrequency(Node<Frequency<TValue,TKey>> newFrequencyNode)
    {
        return this with
        {
            FrequencyNode = newFrequencyNode,
            Frequency = Frequency+1
        };
    }

    public override string ToString()
    {
        return $"Value - {Value}, Key - {Key}, Frequency - {Frequency}";
    }
}