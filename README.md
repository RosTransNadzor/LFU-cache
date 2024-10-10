
# LFU Cache Implementation (Least Frequently Used)

This repository contains an implementation of the **Least Frequently Used (LFU)** cache in C#. The LFU cache is a data structure that evicts the least frequently accessed items first when its capacity is reached. This implementation ensures that operations such as insertion, access, and deletion are performed in **O(1) time complexity**.

## Features

- **Efficient Caching:** Items are stored with an associated frequency count, and the least frequently used items are removed when the cache reaches capacity.
- **O(1) Time Complexity:** Insertion, access, and deletion are optimized to work in constant time using a combination of dictionaries and linked lists.
- **Configurable Policy:** You can configure the cache's addition policy (e.g., whether new items are added to the head or tail of the frequency list).

## Class Overview

### `LFU<TKey, TValue>`

The main class that implements the LFU cache functionality. It takes two type parameters:
- `TKey`: The type of keys used in the LFU cache. It must be non-nullable.
- `TValue`: The type of values stored in the LFU cache.

### Main Methods
- `Insert(TKey key, TValue value)`: Adds a key-value pair to the cache. If the cache reaches its capacity, the least frequently used item is evicted.
- `Access(TKey key)`: Accesses an item by key, increasing its frequency. If the key is not found, an exception is thrown.
- `Get(TKey key)`: Retrieves the value associated with a key without changing its frequency.
- `GetWithFrequency(TKey key)`: Retrieves the value and its frequency for a given key.
- `Remove(TKey key)`: Removes an item by its key from the cache.
- `Clear()`: Clears the cache, removing all items.

### Additional Interfaces Implemented:
- `IDictionary<TKey, TValue>`
- `IReadOnlyDictionary<TKey, TValue>`

These interfaces allow the LFU cache to be used like a standard dictionary with additional features like enumeration.

### Internal Classes
- `LfuItem<TKey, TValue>`: Represents a cache item, storing the key, value, and its current frequency.
- `Frequency<TKey, TValue>`: Represents a frequency node containing items with the same frequency.
- `Node<T>`: A generic node used to store items in the linked list for both frequencies and cache items.

## Time Complexity

- **Insert**: O(1)
- **Access**: O(1)
- **Remove**: O(1)
- **Get**: O(1)

The time complexity is achieved by using a dictionary to store cache items and a linked list to track their frequency efficiently.

## Usage

```csharp
// Example usage of the LFU Cache

var lfuCache = new LFU<int, string>(capacity: 5);

// Inserting items
lfuCache.Insert(1, "Item 1");
lfuCache.Insert(2, "Item 2");

// Accessing items
var value = lfuCache.Access(1);  // "Item 1"

// Checking frequency
var valueFrequency = lfuCache.GetWithFrequency(1);  // { Value = "Item 1", Frequency = 2 }

// Removing items
lfuCache.Remove(2);

// Checking size
Console.WriteLine(lfuCache.Count);  // 1
```

## License

This project is open source and available under the [MIT License](LICENSE).
