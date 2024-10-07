
# LFU<TKey, TValue> Class Documentation

## Overview
The `LFU<TKey, TValue>` class implements a **Least Frequently Used (LFU)** cache, designed to manage items based on their access frequency. This means that items that are least frequently accessed are removed first when the cache reaches its maximum capacity. This class provides **O(1)** complexity for `Insert`, `Access`, and `Delete` operations by utilizing a combination of a **hash table** and a **doubly linked list**.

## Generic Parameters
- **TKey**: Represents the type of keys used to index the cache. `TKey` must be non-nullable (`notnull` constraint).
- **TValue**: Represents the type of values stored in the cache.

## Constructor
```csharp
public LFU(int lfuSize, AddPolicy policy = AddPolicy.AddToTail)
```
### Parameters:
- `lfuSize` (int): The maximum size of the cache. Must be greater than 0.
- `policy` (AddPolicy): A strategy determining whether new items should be added to the tail (least priority) or head (most priority) of the cache. Default is `AddToTail`.

## Key Methods

### Insert(TKey key, TValue value)
```csharp
public void Insert(TKey key, TValue value)
```
Inserts a new key-value pair into the cache. If the cache is full, the least frequently used item is evicted. The insertion is done in **O(1)** time by using a hash table for constant-time lookups and a doubly linked list for managing frequency nodes.

### Access(TKey key)
```csharp
public TValue Access(TKey key)
```
Retrieves the value associated with the provided key and increments its frequency. If the key is not found, an exception is thrown. The operation runs in **O(1)** time.

### Get(TKey key)
```csharp
public TValue Get(TKey key)
```
Returns the value associated with the provided key without changing its frequency. Throws an exception if the key does not exist. Runs in **O(1)** time.

### GetWithFrequency(TKey key)
```csharp
public ValueFrequency<TValue> GetWithFrequency(TKey key)
```
Returns both the value and the current frequency of the key. The operation runs in **O(1)** time.

### DeleteFirstItemWithMinFrequency()
```csharp
private TKey DeleteFirstItemWithMinFrequency()
```
Deletes the least frequently used item in the cache. It looks for the frequency node with the minimum frequency (always located at the head of the frequency list) and removes the item from it. Runs in **O(1)**.

## Performance
All primary operations, including inserting new items, accessing existing items, and removing the least frequently used items, run in **O(1)**. This is achieved by:
- A hash table (`Dictionary<TKey, Node<LfuItem<TValue, TKey>>>`) for constant-time lookups and updates.
- A doubly linked list (`LinkedList<Frequency<TValue, TKey>>`) for managing frequency buckets.
- Efficient item frequency upgrades and evictions by moving items between frequency nodes in **O(1)**.

## Conclusion
This LFU cache is ideal for use cases where **frequent access patterns** are critical, and you need to ensure minimal time complexity for cache operations. With the right balance of hash tables and linked lists, the class maintains **O(1)** complexity for all fundamental operations.
