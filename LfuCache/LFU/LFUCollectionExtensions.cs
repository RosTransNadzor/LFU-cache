using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleTest.LFU;

public partial class LFU<TKey,TValue> : 
    IDictionary<TKey,TValue>,
    IReadOnlyDictionary<TKey,TValue>
{
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var pair in _itemsHashTable)
        {
            var key = pair.Key;
            //it looks terrible.
            var value = pair.Value.Value.Value;
            yield return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Insert(item.Key, item.Value);
    }

    public void Clear()
    {
        _itemsHashTable.Clear();
        _frequencyList.Clear();
        _currentSize = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (!_itemsHashTable.ContainsKey(item.Key))
            return false;

        var node = _itemsHashTable[item.Key];
        return node.Value.Value.Equals(item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
    {
        if (array.Length - index < Count)
            throw new ArgumentException("The array is too small to contain the elements.");

        foreach (var pair in this)
        {
            array[index++] = pair;
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var key = item.Key;
        var value = item.Value;

        if (_itemsHashTable.ContainsKey(key) && _itemsHashTable[key].Value.Value.Equals(value))
        {
            Delete(key);
            return true;
        }

        return false;
    }

    public bool IsReadOnly => false;
    
    /// <summary>
    /// Inserts an item into the LFU cache. If the cache has reached its capacity, the least frequently used item will be removed.
    /// Insertion is performed in O(1) time.
    /// </summary>
    /// <param name="key">The key to insert into the cache.</param>
    /// <param name="value">The value associated with the key.</param>
    /// <exception cref="Exception">Throws an exception if the key already exists in the cache.</exception>
    public void Add(TKey key, TValue value)
    {
        Insert(key,value);
    }

    public bool ContainsKey(TKey key)
    {
        return _itemsHashTable.ContainsKey(key);
    }

    public bool Remove(TKey key)
    { 
        if (!_itemsHashTable.ContainsKey(key))
            return false;
        
        Delete(key);
        return true;
    }

    
    public bool TryGetValue(TKey key,[MaybeNullWhen(false)] out TValue value)
    {
        value = default;
        if (_itemsHashTable.ContainsKey(key))
        {
            value = _itemsHashTable[key].Value.Value;
            return true;
        }

        return false;
    }

    public TValue this[TKey key]
    {
        get => GetRequired(key);
        set => Insert(key, value);
    }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    public ICollection<TKey> Keys => _itemsHashTable.Keys.ToList();

    public ICollection<TValue> Values => new LfuValueCollection(_itemsHashTable);
    
    private class LfuValueCollection : ICollection<TValue>
    {
        private readonly Dictionary<TKey, Node<LfuItem<TKey, TValue>>> _itemsHashTable;

        public LfuValueCollection(Dictionary<TKey, Node<LfuItem<TKey, TValue>>> itemsHashTable)
        {
            _itemsHashTable = itemsHashTable;
        }

        public int Count => _itemsHashTable.Count;

        public bool IsReadOnly => true;

        public void Add(TValue item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(TValue item) => _itemsHashTable.Values.Any(node => node.Value.Value.Equals(item));

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            foreach (var node in _itemsHashTable.Values)
            {
                array[arrayIndex++] = node.Value.Value;
            }
        }

        public bool Remove(TValue item) => throw new NotSupportedException();

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var node in _itemsHashTable.Values)
            {
                yield return node.Value.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

