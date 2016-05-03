using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiniMetrics.Net
{
    public interface IOutbountChannel : IDisposable
    {
        Task ConnectAsync();

        Task WriteAsync(Byte[] data, CancellationToken token);
    }
}