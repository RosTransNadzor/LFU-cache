namespace ConsoleTest.LFU;

/// <summary>
/// A class representing the Least Frequently Used (LFU) cache.
/// This cache stores items with an associated frequency count, where items that are least frequently accessed
/// are removed first when the cache reaches its capacity. The cache supports O(1) time complexity for 
/// inserting, accessing, and deleting elements.
/// </summary>
/// <typeparam name="TKey">The type of keys used in the LFU cache. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of values stored in the LFU cache.</typeparam>
public class LFU<TKey,TValue>
    where TKey : notnull
{
    private readonly int _lfuSize;
    private const int DefaultFrequency = 1;
    private readonly AddPolicy _addPolicy;
    private int _currentSize;
    public int Count => _currentSize;
    private readonly Dictionary<TKey,Node<LfuItem<TValue,TKey>>> _itemsHashTable;
    private readonly LinkedList<Frequency<TValue,TKey>> _frequencyList = new();

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
        
        if (_currentSize == _lfuSize)
        {
            var deletedKey = DeleteFirstItemWithMinFrequency();
            _itemsHashTable.Remove(deletedKey);
        }
        
        Node<Frequency<TValue, TKey>> frequencyNode = FindOrCreateFrequencyNode();
        
        LfuItem<TValue,TKey> item = new()
        {
            Frequency = 1,
            FrequencyNode = frequencyNode,
            Key = key,
            Value = value
        };
        
        Node<LfuItem<TValue,TKey>> node = item;
        
        _itemsHashTable.Add(key,node);
        frequencyNode.Value.AddItem(item);
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
        var lfuItem = node.Value;
       
        var newNode = UpgradeItemFrequency(lfuItem);
        DeleteLfuItemNode(node);

        _itemsHashTable.Remove(key);
        _itemsHashTable[key] = newNode;

        return lfuItem.Value;
    }
    /// <summary>
    /// Retrieves the value associated with the specified key without changing its frequency.
    /// The operation is performed in O(1) time.
    /// </summary>
    /// <param name="key">The key to retrieve from the cache.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="Exception">Throws an exception if the key does not exist in the cache.</exception>
    public TValue Get(TKey key)
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
    public void Delete(TKey key)
    {
        GuardKeyExists(key);

        var lfuItemNode = _itemsHashTable[key];
        _itemsHashTable.Remove(key);
        DeleteLfuItemNode(lfuItemNode);
        _currentSize--;
    }

    private void GuardSizeMoreZero(int size)
    {
        if (size <= 0)
            throw new Exception("size not more than zero");
    }

    private void GuardKeyExists(TKey key)
    {
        if (!_itemsHashTable.ContainsKey(key))
            throw new Exception("key doesn't exists");
    }

    private void GuardKeyNotExits(TKey key)
    {
        if (_itemsHashTable.ContainsKey(key))
            throw new Exception("key already exists");
    }
    
    private Node<Frequency<TValue, TKey>> FindOrCreateFrequencyNode()
    {
        Node<Frequency<TValue, TKey>> result = FindFrequencyNode() ?? CreateFrequencyNode();
        return result;
    }

    /// <summary>
    /// Create Frequency node with 1 frequency and set as head in <see cref="_frequencyList"/>
    /// </summary>
    /// <returns></returns>
    private Node<Frequency<TValue, TKey>> CreateFrequencyNode()
    {
        Node<Frequency<TValue, TKey>> frequencyNode = new Frequency<TValue, TKey>(_addPolicy)
        {
            Freq = DefaultFrequency,
            Items = new()
        };
        
        _frequencyList.AddNodeToHead(frequencyNode);
        return frequencyNode;
    }

    /// <summary>
    /// Find frequency node with 1 frequency
    /// </summary>
    /// <returns></returns>
    private Node<Frequency<TValue, TKey>>? FindFrequencyNode()
    {
        var head = _frequencyList.Head;
        return head is { Value.Freq: DefaultFrequency } ? head : null;
    }
    
    private TKey DeleteFirstItemWithMinFrequency()
    {
        // ! - almost one frequency node exists in list 
        // because lfuSize > 0
        var minFrequencyNode = _frequencyList.Head!;
        var itemsList = minFrequencyNode.Value.Items;
        // ! - frequency node exists if at least one item with that frequency
        var firstLfuItemWithMinFrequency = itemsList.Head!;
        
        DeleteLfuItemNode(firstLfuItemWithMinFrequency);

        return firstLfuItemWithMinFrequency.Value.Key;
    }

    private void DeleteLfuItemNode(Node<LfuItem<TValue,TKey>> node)
    {
        var item = node.Value;
        var frequencyNode = item.FrequencyNode;
        
        frequencyNode.Value.Items.RemoveNode(node);
        // no items with this frequency
        if(frequencyNode.Value.Items.Head is null)
            _frequencyList.RemoveNode(frequencyNode);
    }

    private Node<LfuItem<TValue,TKey>> UpgradeItemFrequency(LfuItem<TValue,TKey> lfuItem)
    {
        var frequencyNode = lfuItem.FrequencyNode;
        Node<Frequency<TValue,TKey>> newFrequencyNode;
        // increase item's frequency
        var next = frequencyNode.Next;
        if (next is not null && next.Value.Freq == frequencyNode.Value.Freq + 1)
            newFrequencyNode = next;
        else
            newFrequencyNode = new Frequency<TValue, TKey>(_addPolicy)
            {
                Freq = lfuItem.Frequency + 1,
                Items = new()
            };
        
        var newItem = lfuItem.UpgradeFrequency(newFrequencyNode);
        
        var newNode = new Node<LfuItem<TValue, TKey>>
        {
            Value = newItem
        };
        newFrequencyNode.Value.AddItem(newNode);
        
        if(newFrequencyNode != next)
            _frequencyList.AddNodeAfter(frequencyNode,newFrequencyNode);

        return newNode;
    }
}