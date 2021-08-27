using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Entity.Ranking
{
    internal sealed class LruCache<TKey, TValue>
        where TKey : notnull
    {
        private readonly LinkedList<KeyValuePair<TKey, TValue>> list = new();
        private readonly ConcurrentDictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> dictionary;
        private readonly int capacity;
        private int count;

        public LruCache(int capacity)
        {
            this.capacity = capacity;
            dictionary = new ConcurrentDictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        }

        public LruCache(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.capacity = capacity;
            dictionary = new ConcurrentDictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(comparer);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (!dictionary.TryGetValue(key, out var node))
            {
                value = default;
                return false;
            }
            else
            {
                // Only lock if we have to modified the LRU
                if (node != list.Last)
                {
                    lock (list)
                    {
                        list.Remove(node);
                        list.AddLast(node);
                    }
                }

                value = node.Value.Value;
                return true;
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            var node = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
            if (dictionary.TryAdd(key, node))
            {
                lock (list)
                {
                    if (count > capacity)
                    {
                        var f = list.First;
                        if (f != null)
                        {
                            list.Remove(f);
                            dictionary.TryRemove(f.Value.Key, out _);
                            (f.Value.Value as IDisposable)?.Dispose();
                        }
                    }
                    else
                    {
                        ++count;
                    }

                    list.AddLast(node);
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
                list.Remove(value);
                --count;
            }

            (value.Value.Value as IDisposable)?.Dispose();

            return true;
        }
    }
}
