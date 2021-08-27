using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combination.Collections.Tests
{
    public class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose() => IsDisposed = true;
    }
}
