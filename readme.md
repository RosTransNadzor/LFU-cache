
# LFU Cache Implementation in C#

This repository contains an implementation of the **Least Frequently Used (LFU)** cache algorithm in C#. The LFU cache is a data structure that stores a limited number of key-value pairs and removes the least frequently accessed items when it reaches its capacity.

## Key Features
- **O(1) Time Complexity**: The implementation provides O(1) time complexity for the following operations:
  - Insertion of key-value pairs.
  - Accessing elements.
  - Deletion of elements.
- **Frequency-Based Eviction**: The cache automatically removes the least frequently accessed item when it reaches its size limit.
- **Flexible Insertion Policy**: Users can specify how new items are added to the frequency list (e.g., add to the head or tail).

## Class Overview

### `LFU<TKey, TValue>`

A generic LFU cache class that supports keys of type `TKey` and values of type `TValue`. It provides methods for inserting, accessing, retrieving, and deleting items.

### Type Parameters:
- `TKey`: The type of keys used in the LFU cache. Must be non-nullable.
- `TValue`: The type of values stored in the LFU cache.

### Constructor:
```csharp
public LFU(int lfuSize, AddPolicy policy = AddPolicy.AddToTail)
```
- **lfuSize**: The maximum capacity of the cache.
- **policy**: Optional parameter to specify how new items are added (either to the head or tail of the frequency list).

## Public Methods

### `Insert(TKey key, TValue value)`
Inserts a new key-value pair into the cache. If the cache is full, it removes the least frequently accessed item. Insertion is performed in **O(1)** time.
```csharp
public void Insert(TKey key, TValue value)
```

### `Access(TKey key)`
Accesses the value associated with the given key and increases its frequency count. Returns the value in **O(1)** time.
```csharp
public TValue Access(TKey key)
```

### `Get(TKey key)`
Returns the value associated with the given key without altering its frequency. Retrieval is done in **O(1)** time.
```csharp
public TValue Get(TKey key)
```

### `GetWithFrequency(TKey key)`
Returns both the value and the frequency of the specified key.
```csharp
public ValueFrequency<TValue> GetWithFrequency(TKey key)
```

### `Delete(TKey key)`
Removes the key-value pair from the cache. The operation is performed in **O(1)** time.
```csharp
public void Delete(TKey key)
```

## Example Usage

```csharp
var lfuCache = new LFU<int, string>(capacity: 3);

lfuCache.Insert(1, "A");
lfuCache.Insert(2, "B");
lfuCache.Insert(3, "C");

// Access some items
lfuCache.Access(1); // increases frequency of key 1
lfuCache.Access(2); // increases frequency of key 2

// Insert new item, key 3 will be removed as it is the least frequently used.
lfuCache.Insert(4, "D");

Console.WriteLine(lfuCache.Get(4)); // Output: "D"
```

## How It Works

The LFU cache uses a **doubly linked list** to maintain frequency groups. Each group contains a list of items with the same frequency. The least frequently accessed item is stored in the head of the list and is removed when the cache exceeds its capacity.

## Time Complexity
All major operations (insert, access, get, delete) are performed in constant **O(1)** time.

## License
This project is licensed under the MIT License.
