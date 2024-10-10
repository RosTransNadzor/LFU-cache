namespace ConsoleTest.LFU;

/// <summary>
/// A class representing the Least Frequently Used (LFU) cache.
/// This cache stores items with an associated frequency count, where items that are least frequently accessed
/// are removed first when the cache reaches its capacity. The cache supports O(1) time complexity for 
/// inserting, accessing, and deleting elements.
/// See more http://dhruvbird.com/lfu.pdf
/// </summary>
/// <typeparam name="TKey">The type of keys used in the LFU cache. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of values stored in the LFU cache.</typeparam>
public partial class LFU<TKey,TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly int _lfuSize;
    private const int DefaultFrequency = 1;
    private readonly AddPolicy _addPolicy;
    private int _currentSize;
    public int Count => _currentSize;
    private readonly Dictionary<TKey,Node<LfuItem<TKey,TValue>>> _itemsHashTable;
    private readonly LinkedList<Frequency<TKey,TValue>> _frequencyList = new();

    public LFU(int lfuSize,AddPolicy policy = AddPolicy.AddToTail)
    {
        GuardSizeMoreZero(lfuSize);
        _lfuSize = lfuSize;
        _addPolicy = policy;
        _itemsHashTable = new(_lfuSize);
    }
    
    /// <summary>
    /// Inserts an item into the LFU cache. If the cache has reached its capacity, the least frequently used item will be removed.
    /// Insertion is performed in O(1) time.
    /// </summary>
    /// <param name="key">The key to insert into the cache.</param>
    /// <param name="value">The value associated with the key.</param>
    /// <exception cref="Exception">Throws an exception if the key already exists in the cache.</exception>
    public void Insert(TKey key,TValue value)
    {
        GuardKeyNotExits(key);

        Node<LfuItem<TKey, TValue>> node;
        Node<Frequency<TKey, TValue>> frequencyNode = FindDefaultFrequencyNode();
        
        LfuItem<TKey,TValue> item = new()
        {
            Frequency = 1,
            FrequencyNode = frequencyNode,
            Key = key,
            Value = value
        };

        // we reuse old node to reduce allocations
        if (_currentSize == _lfuSize)
        {
            node = Evict();
            node.Value = item;
        }
        else
        {
            node = new Node<LfuItem<TKey, TValue>>
            {
                Value = item
            };
        }
        _itemsHashTable.Add(key,node);
        frequencyNode.Value.AddItem(node);
        _currentSize = Math.Min(_currentSize + 1, _lfuSize);
    }
    
    /// <summary>
    /// Accesses the item associated with the specified key, increasing its access frequency.
    /// The operation is performed in O(1) time.
    /// </summary>
    /// <param name="key">The key to access in the cache.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="Exception">Throws an exception if the key does not exist in the cache.</exception>
    public TValue Access(TKey key)
    {
        GuardKeyExists(key);

        var node = _itemsHashTable[key];
        
        DeleteLfuItemNode(node);
        UpgradeItemFrequency(node);
        
        _itemsHashTable.Remove(key);
        _itemsHashTable[key] = node;

        var lfuItem = node.Value;
        return lfuItem.Value;
    }
    /// <summary>
    /// Retrieves the value associated with the specified key without changing its frequency.
    /// The operation is performed in O(1) time.
    /// </summary>
    /// <param name="key">The key to retrieve from the cache.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="Exception">Throws an exception if the key does not exist in the cache.</exception>
    public TValue? Get(TKey key)
    {
        if (!_itemsHashTable.ContainsKey(key))
            return default;
        
        var lfuItem = _itemsHashTable[key].Value;
        return lfuItem.Value;
    }
    private TValue GetRequired(TKey key)
    {
        GuardKeyExists(key);
        
        var lfuItem = _itemsHashTable[key].Value;
        return lfuItem.Value;
    }
    /// <summary>
    /// Retrieves both the value and its access frequency associated with the specified key.
    /// The operation is performed in O(1) time.
    /// </summary>
    /// <param name="key">The key to retrieve the value and frequency for.</param>
    /// <returns>A struct containing the value and its access frequency.</returns>
    /// <exception cref="Exception">Throws an exception if the key does not exist in the cache.</exception>
    public ValueFrequency<TValue> GetWithFrequency(TKey key)
    {
        GuardKeyExists(key);

        var lfuItem = _itemsHashTable[key].Value;
        return new ValueFrequency<TValue>
        {
            Frequency = lfuItem.Frequency,
            Value = lfuItem.Value
        };
    }
    /// <summary>
    /// Deletes the item associated with the specified key from the cache. 
    /// The operation is performed in O(1) time.
    /// </summary>
    /// <param name="key">The key to remove from the cache.</param>
    /// <exception cref="Exception">Throws an exception if the key does not exist in the cache.</exception>
    private void Delete(TKey key)
    {
        var lfuItemNode = _itemsHashTable[key];
        _itemsHashTable.Remove(key);
        DeleteLfuItemNode(lfuItemNode);
        _currentSize--;
    }

    private void GuardSizeMoreZero(int size)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException($"size ({size})  must be more zero");
    }

    private void GuardKeyExists(TKey key)
    {
        if (!_itemsHashTable.ContainsKey(key))
            throw new ArgumentException($"value with key {key} doesn't exists");
    }

    private void GuardKeyNotExits(TKey key)
    {
        if (_itemsHashTable.ContainsKey(key))
            throw new ArgumentException($"key already exists {key}");
    }
    
    private Node<Frequency<TKey,TValue>> FindDefaultFrequencyNode()
    {
        var head = _frequencyList.Head;
        if (head is { Value.Freq: DefaultFrequency })
            return head;
        
        var frequency = new Frequency<TKey,TValue>(_addPolicy)
        {
            Freq = DefaultFrequency,
            Items = new()
        };
        Node<Frequency<TKey, TValue>> defaultNode = new Node<Frequency<TKey, TValue>>
        {
            Value = frequency
        };
        
        _frequencyList.AddNodeToHead(defaultNode);
        return defaultNode;
    }
    private Node<LfuItem<TKey,TValue>> Evict()
    {
        // ! - almost one frequency node exists in list 
        // because lfuSize > 0
        var minFrequencyNode = _frequencyList.Head!;
        var itemsList = minFrequencyNode.Value.Items;
        // ! - frequency node exists if at least one item with that frequency
        var firstLfuItemWithMinFrequency = itemsList.Head!;
        
        DeleteLfuItemNode(firstLfuItemWithMinFrequency);

        var deletedKey =  firstLfuItemWithMinFrequency.Value.Key;
        _itemsHashTable.Remove(deletedKey);

        return firstLfuItemWithMinFrequency;
    }

    private void DeleteLfuItemNode(Node<LfuItem<TKey,TValue>> node)
    {
        var item = node.Value;
        var frequencyNode = item.FrequencyNode;
        
        frequencyNode.Value.Items.RemoveNode(node);
        // no items with this frequency
        if(frequencyNode.Value.Items.Head is null)
            _frequencyList.RemoveNode(frequencyNode);
    }

    private void UpgradeItemFrequency(Node<LfuItem<TKey,TValue>> node)
    {
        var lfuItem = node.Value;
        var frequencyNode = lfuItem.FrequencyNode;
        
        Node<Frequency<TKey,TValue>> nextFrequencyNode = FindNextFrequency(frequencyNode);
        // increase item's frequency
        var newItem = lfuItem.UpgradeFrequency(nextFrequencyNode);
        
        node.Value = newItem;
        nextFrequencyNode.Value.AddItem(node);
        
        if(nextFrequencyNode != frequencyNode.Next)
            _frequencyList.AddNodeAfter(frequencyNode,nextFrequencyNode);
    }

    /// <summary>
    /// Finds node that has frequency = old node's frequency + 1
    /// </summary>
    /// <param name="node">node with old frequency</param>
    /// <returns></returns>
    private Node<Frequency<TKey, TValue>> FindNextFrequency(Node<Frequency<TKey, TValue>> node)
    {
        Node<Frequency<TKey, TValue>> result;

        if (node.Next is not null && node.Next.Value.Freq == node.Value.Freq + 1)
            result = node.Next;
        
        // we reuse node with frequency with 0 items
        else if (node.Value.Freq != DefaultFrequency && node.Value.Items.Length == 0)
        {
            node.Value = node.Value with { Freq = node.Value.Freq + 1 };
            result = node;
        }
        else
        {
            result = new()
            {
                Value = new Frequency<TKey, TValue>(_addPolicy)
                {
                    Freq = node.Value.Freq + 1,
                    Items = new()
                } 
            };
        }

        return result;
    }
}