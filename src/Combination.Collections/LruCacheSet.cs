using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public sealed class LruCacheSet<TKey> where TKey : notnull
{
    private readonly LinkedList<TKey> list = new();

    private readonly ConcurrentDictionary<TKey, LinkedListNode<TKey>> dictionary;

    private readonly int capacity;

    public int Count { get; private set; }

    private readonly bool disposeOnRemove;

    public LruCacheSet(int capacity, bool disposeOnRemove = true)
    {
        this.capacity = capacity;
        this.disposeOnRemove = disposeOnRemove;
        dictionary = new ConcurrentDictionary<TKey, LinkedListNode<TKey>>();
    }

    public LruCacheSet(int capacity, IEqualityComparer<TKey> comparer, bool disposeOnRemove = true)
    {
        this.capacity = capacity;
        this.disposeOnRemove = disposeOnRemove;
        dictionary = new ConcurrentDictionary<TKey, LinkedListNode<TKey>>(comparer);
    }

    public bool ContainsKey(TKey key)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            return false;
        }

        if (value != list.First)
        {
            lock (list)
            {
                if (value.List != null)
                {
                    list.Remove(value);
                    list.AddFirst(value);
                }
            }
        }

        return true;
    }

    public bool TryGetKey(TKey key, [MaybeNullWhen(false)] out TKey storedKey)
    {
        if (!dictionary.TryGetValue(key, out var value2))
        {
            storedKey = default;
            return false;
        }

        if (value2 != list.First)
        {
            lock (list)
            {
                if (value2.List != null)
                {
                    list.Remove(value2);
                    list.AddFirst(value2);
                }
            }
        }

        storedKey = value2.Value;
        return true;
    }

    public bool TryAdd(TKey key)
    {
        var linkedListNode = new LinkedListNode<TKey>(key);
        if (dictionary.TryAdd(key, linkedListNode))
        {
            lock (list)
            {
                if (++Count > capacity)
                {
                    Count--;
                    var last = list.Last;
                    if (last != null)
                    {
                        list.Remove(last);
                        dictionary.TryRemove(last.Value, out var _);
                        if (disposeOnRemove)
                        {
                            (last.Value as IDisposable)?.Dispose();
                        }
                    }
                }

                list.AddFirst(linkedListNode);
            }

            return true;
        }

        return false;
    }

    public bool TryRemove(TKey key)
    {
        if (!dictionary.TryRemove(key, out var value))
        {
            return false;
        }

        lock (list)
        {
            if (value.List != null)
            {
                list.Remove(value);
                Count--;
            }
        }

        if (disposeOnRemove)
        {
            (value.Value as IDisposable)?.Dispose();
        }

        return true;
    }
}
