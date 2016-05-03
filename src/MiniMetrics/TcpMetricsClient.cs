using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MiniMetrics.Extensions;

namespace MiniMetrics
{
    // TODO: throw ObjectDisposedException
    // TODO: autorecovery
    public class TcpMetricsClient : IMetricsClient
    {
        private static readonly TimeSpan BreathTime = TimeSpan.FromSeconds(5d);

        private readonly ConcurrentQueue<String> _messages = new ConcurrentQueue<String>();
        private readonly CancellationTokenSource _cts;
        private readonly TcpClient _client;
        private readonly Func<Encoding> _encodingFactory;

        public event EventHandler<MessageSentEventArgs> OnMessageSent;

        private Int32 _disposed;

        public static Task<IMetricsClient> StartAsync(String hostname,
                                                      Int32 port,
                                                      Func<Encoding> encodingFactory = null)
        {
            if (hostname == null)
                throw new ArgumentNullException(nameof(hostname));

            if (hostname.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(hostname));

            return Dns.GetHostEntryAsync(hostname)
                      .ContinueWith(_ =>
                                    {
                                        _.ThrowOnError();

                                        if (_.Result.AddressList.Length == 0)
                                            throw new InvalidOperationException("unable to find an ip address for specified hostname");

                                        return StartAsync(_.Result.AddressList[0], port);
                                    })
                      .Unwrap();
        }

        public static Task<IMetricsClient> StartAsync(IPAddress address,
                                                      Int32 port,
                                                      Func<Encoding> encodingFactory = null)
        {
            var client = new TcpClient { ExclusiveAddressUse = false };

            return client.ConnectAsync(address, port)
                         .ContinueWith(_ =>
                                       {
                                           _.ThrowOnError();
                                           return (IMetricsClient)new TcpMetricsClient(client,
                                                                                       encodingFactory ?? (() => new UTF8Encoding(true)));
                                       });
        }

        private TcpMetricsClient(TcpClient client, Func<Encoding> encodingFactory)
        {
            _client = client;
            _encodingFactory = encodingFactory;
            _cts = new CancellationTokenSource();

            BackgroundWorkAsync(_cts.Token);
        }

        public void Send(String message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            _messages.Enqueue(message);
        }

        public void Dispose()
        {
            var i = Interlocked.CompareExchange(ref _disposed, 1, 0);

            if (i == 0)
                return;

            try
            {
                _cts.Cancel();
                _client?.Close();
            }
            catch { }
        }

        private Task BackgroundWorkAsync(CancellationToken token)
        {
            String message;

            if (!_messages.TryDequeue(out message))
                return Task.Delay(BreathTime, token)
                           .ContinueWith(_ => BackgroundWorkAsync(token), token)
                           .Unwrap();

            var bytes = _encodingFactory().GetBytes(message);
            var stream = _client.GetStream();

            return Task.Factory
                       .FromAsync(stream.BeginWrite, stream.EndWrite, bytes, 0, bytes.Length, null)
                       .ContinueWith(_ =>
                                     {
                                         stream.Dispose();
                                     
                                         if (_.Exception != null)
                                             throw _.Exception.GetBaseException();
                                     
                                         if (token.IsCancellationRequested)
                                             throw new Exception("task has been cancelled.");
                                     
                                         var temp = Interlocked.CompareExchange(ref OnMessageSent, null, null);
                                         temp?.Invoke(this, new MessageSentEventArgs(message));
                                     
                                         return BackgroundWorkAsync(token);
                                     },
                                     token)
                       .Unwrap();
        }
    }
}