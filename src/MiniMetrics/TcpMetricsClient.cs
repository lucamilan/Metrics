using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MiniMetrics.Extensions;
using MiniMetrics.Net;

namespace MiniMetrics
{
    public class TcpMetricsClient : IMetricsClient
    {
        private static readonly TimeSpan DefaultBreathTime = TimeSpan.FromSeconds(5d);

        protected readonly ConcurrentQueue<String> Queue = new ConcurrentQueue<String>();

        private readonly CancellationTokenSource _cts;
        private readonly IOutbountChannel _channel;
        private readonly TimeSpan _breathTime;
        private readonly IFormatter _formatter;
        private readonly Func<Encoding> _encodingFactory;

        public event EventHandler<MessageSentEventArgs> OnMessageSent;

        private Int32 _disposed;

        public static Task<IMetricsClient> StartAsync(IOutbountChannel channel,
                                                      IFormatter formatter = null,
                                                      Func<Encoding> encodingFactory = null)
        {
            return StartAsync(channel, DefaultBreathTime, formatter, encodingFactory);
        }

        public static Task<IMetricsClient> StartAsync(IOutbountChannel channel,
                                                      TimeSpan breathTime,
                                                      IFormatter formatter = null,
                                                      Func<Encoding> encodingFactory = null)
        {
            return channel.ConnectAsync()
                          .ContinueWithOrThrow(_ => (IMetricsClient)new TcpMetricsClient(channel,
                                                                                         breathTime,
                                                                                         formatter ?? new DefaultFormatter(),
                                                                                         encodingFactory ?? (() => new UTF8Encoding(true))));
        }

        protected TcpMetricsClient(IOutbountChannel channel,
                                   TimeSpan breathTime,
                                   IFormatter formatter,
                                   Func<Encoding> encodingFactory)
        {
            _channel = channel;
            _breathTime = breathTime;
            _formatter = formatter;
            _encodingFactory = encodingFactory;
            _cts = new CancellationTokenSource();

            BackgroundWorkAsync(_cts.Token);
        }

        public void Send<TValue>(String key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var i = Interlocked.CompareExchange(ref _disposed, 0, 0);

            if (i == 1)
                throw new ObjectDisposedException(GetType().Name);

            Queue.Enqueue(_formatter.Format(key, value));
        }

        public void Dispose()
        {
            var i = Interlocked.CompareExchange(ref _disposed, 1, 0);

            if (i == 1)
                return;

            DisposeInternal();
        }

        protected virtual void DisposeInternal()
        {
            try
            {
                _cts.Cancel();
                _channel?.Dispose();
            }
            catch { }
        }

        private void BackgroundWorkAsync(CancellationToken token)
        {
            BuildTask(token).ContinueWithOrThrow(_ => BackgroundWorkAsync(token), token);
        }

        private Task BuildTask(CancellationToken token)
        {
            String message;

            return Queue.TryDequeue(out message)
                       ? _channel.WriteAsync(_encodingFactory().GetBytes(message), token)
                                 .ContinueWithOrThrow(_ => RaiseOnMessageSent(message), token)
                       : Task.Delay(_breathTime, token);
        }

        private void RaiseOnMessageSent(String message)
        {
            var temp1 = Interlocked.CompareExchange(ref OnMessageSent, null, null);
            temp1?.Invoke(this, new MessageSentEventArgs(message));
        }
    }
}