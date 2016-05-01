using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniMetrics
{
    internal sealed class DisposableAction : IDisposable
    {
        private Action _disposeAction;

        public DisposableAction(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            //  Interlocked allows the continuation to be executed only once
            var continuation = Interlocked.Exchange(ref _disposeAction, null);

            continuation?.Invoke();
        }
    }
}