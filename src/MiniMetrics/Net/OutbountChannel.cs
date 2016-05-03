using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MiniMetrics.Extensions;

namespace MiniMetrics.Net
{
    public class OutbountChannel : IOutbountChannel
    {
        protected readonly TcpClient Client;
        private readonly IPAddress _address;
        private readonly Int32 _port;

        public OutbountChannel(TcpClient client, IPAddress address, Int32 port)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (address == null)
                throw new ArgumentNullException(nameof(address));

            Client = client;
            _address = address;
            _port = port;
        }

        public Task ConnectAsync()
        {
            return Client.ConnectAsync(_address, _port);
        }

        public void Dispose()
        {
            Client?.Close();
        }

        public virtual Task WriteAsync(Byte[] data, CancellationToken token)
        {
            var stream = Client.GetStream(); // TODO: try catch
            return Task.Factory
                       .FromAsync(stream.BeginWrite,
                                  stream.EndWrite,
                                  data,
                                  0,
                                  data.Length,
                                  stream) // TODO: it shoud work by "chunks".
                       .ContinueWith(_ =>
                                     {
                                         (_.AsyncState as Stream)?.Dispose();
                                         stream.Dispose();
                                         _.ThrowOnError();
                                     },
                                     token);
        }
    }
}