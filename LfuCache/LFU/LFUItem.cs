namespace ConsoleTest.LFU;

/// <summary>
/// Lfu items with same frequency form linked list
/// </summary>
/// <typeparam name="TValue">type of value in item</typeparam>
/// <typeparam name="TKey">type of key in item</typeparam>
public readonly struct LfuItem<TKey,TValue>
{
    public required int Frequency { get; init; }
    public required TValue Value { get; init; }
    public required TKey Key { get; init; }
    
    public required Node<Frequency<TKey,TValue>> FrequencyNode { get; init; }
    
    public LfuItem<TKey,TValue> UpgradeFrequency(Node<Frequency<TKey,TValue>> newFrequencyNode)
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