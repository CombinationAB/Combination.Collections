# Combination.Collections

Useful collection primitives that are missing from the .NET standard library.

# Collection types provided by this library

## `LruCache` - A threadsafe LRU cache with optional `IDisposable` support

Usage example

    // Creates an LRU cache with a capacity of 10
    var lru = new LruCache<string, MyValue>(10); 
    
    // Adds a value for key "key". If key already exists, returns false and leaves the collection unmodified.
    var wasAdded = lru.TryAdd("key", value); 
    
    // Gets a value from the LRU, marking it as recent
    if(lru.TryGetValue("key", out var value))
    {
      // If true, we found it in the cache
    }
    else
    {
      // If false, not in the cache
    }
    
    // Remove an item
    var wasRemoved = lru.TryRemove("key")
