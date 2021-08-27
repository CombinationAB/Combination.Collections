using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Combination.Collections.Tests
{
    public class LruCacheTests
    {
        [Fact]
        public void Can_Add()
        {
            LruCache<string, int> cache = new(1);
            cache.TryAdd("test", 5);
            cache.TryGetValue("test", out var x);
            Assert.Equal(5, x);
        }

        [Fact]
        public void Can_Get()
        {
            LruCache<string, int> cache = new(1);
            cache.TryAdd("test", 5);
            var result = cache.TryGetValue("test", out var x);
            Assert.Equal(5, x);
            Assert.True(result);
        }

        [Fact]
        public void Can_Remove()
        {
            LruCache<string, int> cache = new(1);
            cache.TryAdd("test", 5);
            var result = cache.TryRemove("test");
            Assert.True(result);
            Assert.False(cache.TryGetValue("test", out var _));
        }

        [Fact]
        public void Drops_Oldest_After_Exceeding_Capacity()
        {
            LruCache<string, int> cache = new(2);
            cache.TryAdd("testA", 5);
            cache.TryAdd("testB", 5);
            cache.TryAdd("testC", 5);

            Assert.False(cache.TryGetValue("testA", out var _));
        }

        [Fact]
        public void Usage_Prevents_Oldest_From_Being_Dropped()
        {
            LruCache<string, int> cache = new(2);
            cache.TryAdd("testA", 5);
            cache.TryAdd("testB", 5);

            var result = cache.TryGetValue("testA", out var x);
            Assert.True(result);
            Assert.Equal(5, x);

            cache.TryAdd("testC", 5);

            Assert.True(cache.TryGetValue("testA", out var _));
        }

        [Fact]
        public void Age_Is_Preserved_When_Choosing_What_To_Drop_Due_To_Capacity()
        {
            LruCache<string, int> cache = new(2);
            cache.TryAdd("testA", 5);
            cache.TryAdd("testB", 5);

            cache.TryGetValue("testA", out var x);

            cache.TryAdd("testC", 5);

            Assert.False(cache.TryGetValue("testB", out var _));
            Assert.True(cache.TryGetValue("testA", out var _));
            Assert.True(cache.TryGetValue("testC", out var _));
        }

        [Fact]
        public void Only_Dispose_Removed_Objects_When_Configured_To()
        {
            LruCache<string, Disposable> cache = new(2);
            var myDisposable = new Disposable();
            cache.TryAdd("test", myDisposable);
            cache.TryRemove("test");

            Assert.True(myDisposable.IsDisposed);

            cache = new(2, false);
            myDisposable = new Disposable();
            cache.TryAdd("test", myDisposable);
            cache.TryRemove("test");

            Assert.False(myDisposable.IsDisposed);
        }

        [Fact]
        public void Will_Not_Always_Push_Out_After_Exceeding_Capacity()
        {
            LruCache<string, int> cache = new(2);
            
            cache.TryAdd("a", 1);
            cache.TryAdd("b", 2);
            cache.TryAdd("c", 3);

            // State should be [b, c] now
            cache.TryRemove("b");

            // State should be [c] now 
            cache.TryAdd("d", 4);

            // State should be [c, d] now and "c" should not have been removed by "d"
            Assert.True(cache.TryGetValue("c", out var _));
        }
    }
}
