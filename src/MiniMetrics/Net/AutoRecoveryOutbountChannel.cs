using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MiniMetrics.Net
{
    public class AutoRecoveryOutbountChannel : OutbountChannel
    {
        private readonly TimeSpan _recoverySlice;

        public AutoRecoveryOutbountChannel(TcpClient client, IPAddress address, Int32 port)
            : this(client, address, port, TimeSpan.FromSeconds(5d))
        {
        }

        public AutoRecoveryOutbountChannel(TcpClient client,
                                           IPAddress address,
                                           Int32 port,
                                           TimeSpan recoverySlice)
            : base(client, address, port)
        {
            _recoverySlice = recoverySlice;
        }

        public override Task WriteAsync(Byte[] data, CancellationToken token)
        {
            if (!Client.Connected)
                // TODO: what about exposing a .NET event?
                return Task.Delay(_recoverySlice, token)
                           .ContinueWith(_ => ConnectAsync(), token)
                           .Unwrap();

            return base.WriteAsync(data, token);
        }
    }
}