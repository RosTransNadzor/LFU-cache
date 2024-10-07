﻿namespace ConsoleTest.LFU;

public enum AddPolicy : byte
{
    AddToTail,
    AddToHead
}
/// <summary>
/// Keeps linked list lfuItems with concrete frequency
/// </summary>
/// <typeparam name="TValue">type of value in lfuItems</typeparam>
/// <typeparam name="TKey">type of key in lfuItems</typeparam>
public readonly struct Frequency<TValue,TKey>
{
    private readonly AddPolicy _addPolicy;

    public Frequency(AddPolicy policy = AddPolicy.AddToTail)
    {
        _addPolicy = policy;
    }
    public required int Freq { get; init; }
    public required LinkedList<LfuItem<TValue, TKey>> Items { get; init; }
    public void AddItem(Node<LfuItem<TValue,TKey>> lfuItem)
    {
        if(_addPolicy == AddPolicy.AddToTail)
            Items.AddNodeToTail(lfuItem);
        else
            Items.AddNodeToHead(lfuItem);
    }

    public override string ToString()
    {
        return $"Frequency - {Freq},Items length - {Items.Length}";
    }
}