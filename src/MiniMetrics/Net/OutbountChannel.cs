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

        public static OutbountChannelFactory From(IPAddress address, Int32 port)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            return new OutbountChannelFactory(address, port);
        }

        public static OutbountChannelFactory From(String hostname, Int32 port)
        {
            if (hostname == null)
                throw new ArgumentNullException(nameof(hostname));

            if (hostname.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(hostname));

            return Dns.GetHostEntryAsync(hostname)
                      .ContinueWithOrThrow(_ =>
                                           {
                                               if (_.Result.AddressList.Length == 0)
                                                   throw new InvalidOperationException("unable to find an ip address for specified hostname");

                                               return new OutbountChannelFactory(_.Result.AddressList[0],
                                                                                 port);
                                           })
                      .Result; // NOTE/ACK: it's not good blocking this call, but it should be
                               //           called just once and that keeps build interface
                               //           simpler.
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
            Stream stream;

            try { stream = Client.GetStream(); }
            catch (Exception exception) { return Task.FromException(exception); }

            return Task.Factory
                       .FromAsync(stream.BeginWrite,
                                  stream.EndWrite,
                                  data,
                                  0,
                                  data.Length,
                                  stream) // TODO: it shoud work by "chunks".
                       .ContinueWithOrThrow(_ => (_.AsyncState as Stream)?.Dispose(), token);
        }
    }
}