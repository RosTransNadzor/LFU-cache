namespace ConsoleTest.LFU;

public class Node<TValue>
{
    public required TValue Value { get; set; }
    public Node<TValue>? Next { get; set; }
    public Node<TValue>? Prev { get; set; }

    public static implicit operator Node<TValue>(TValue value)
    {
        return new Node<TValue>
        {
            Value = value
        };
    }

    public override string ToString()
    {
        return $"Value - {Value}";
    }
}