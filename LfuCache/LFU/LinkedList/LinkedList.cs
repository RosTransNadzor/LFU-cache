namespace ConsoleTest.LFU;

/// <summary>
/// Simple linked list realization
/// </summary>
/// <typeparam name="TValue">type of value in nodes</typeparam>
public class LinkedList<TValue>
{
    public Node<TValue>? Head { get; set; }
    public Node<TValue>? Tail { get; set; }
    public int Length => GetLength();

    private int GetLength()
    {
        int length = 0;
        var current = Head;
        while (current is not null)
        {
            current = current.Next;
            length++;
        }

        return length;
    }
    public void AddNodeToHead(Node<TValue> node)
    {
        var oldHead = Head;
        Head = node;
        if (oldHead is null)
            Tail = node;
        else
        {
            oldHead.Prev = node;
            node.Next = oldHead;
        }
    }
    public void AddNodeToTail(Node<TValue> node)
    {
        if (Head is null)
        {
            Head = node;
            Tail = node;
        }
        else
        {
            // invariant: if head is not null -> tail is not null
            Tail!.Next = node;
            node.Prev = Tail;
            Tail = node;
        }
    }

    public void AddNodeAfter(Node<TValue> beforeNode, Node<TValue> newNode)
    {
        var beforeNext = beforeNode.Next;
        beforeNode.Next = newNode;
        newNode.Prev = beforeNode;
        newNode.Next = beforeNext;
        if(beforeNext is not null)
            beforeNext.Prev = newNode;
        if (ReferenceEquals(Tail, beforeNode))
            Tail = newNode;
    }
    public void RemoveNode(Node<TValue> node)
    {
        var nextNode = node.Next;
        var previousNode = node.Prev;

        // Соединяем соседние узлы
        if (nextNode != null)
            nextNode.Prev = previousNode;

        if (previousNode != null)
            previousNode.Next = nextNode;

        // update head and tail 
        UpdateHeadAndTail(node);
    }

    private void UpdateHeadAndTail(Node<TValue> node)
    {
        if (ReferenceEquals(node, Head))
            Head = node.Next;

        if (ReferenceEquals(node, Tail))
            Tail = node.Prev;
    }
}