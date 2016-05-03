using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MiniMetrics.Net;
using Xunit;

namespace MiniMetrics.Tests
{
    public class TcpMetricsClientTests : IDisposable
    {
        private readonly TcpListener _listener;

        public TcpMetricsClientTests()
        {
            _listener = new TcpListener(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort);
            _listener.Start();
        }

        [Fact]
        public void SendMessage()
        {
            const String message = "test";

            var @event = new ManualResetEvent(false);
            String result = null;

            TcpMetricsClient.StartAsync(OutbountChannel.From(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort)
                                                       .BuildAutoRecoverable())
                            .ContinueWith(_ =>
                                          {
                                              _.Result.OnMessageSent += (sender, args) =>
                                                                        {
                                                                            result = message;
                                                                            @event.Set();
                                                                        };
                                              _.Result.Send(message);
                                          });

            if (!@event.WaitOne(TimeSpan.FromSeconds(10d)))
                throw new Exception("timed out");

            Assert.Equal(message, result);
        }

        [Fact]
        public void SendingAfterDisposing()
        {
            var wrapper = new TcpMetricsClientWrapper(null, TimeSpan.FromMilliseconds(-1d), null);
            wrapper.Send("a-message");
            wrapper.Dispose();
            Assert.Throws<ObjectDisposedException>(() => wrapper.Send("another-message"));
            wrapper.Dispose();
            Assert.Equal(1, wrapper.DisposeCount);
        }

        public void Dispose()
        {
            _listener.Stop();
        }

        internal class TcpMetricsClientWrapper : TcpMetricsClient
        {
            internal Int32 DisposeCount;

            public TcpMetricsClientWrapper(IOutbountChannel channel,
                                           TimeSpan breathTime,
                                           Func<Encoding> encodingFactory)
                : base(channel, breathTime, encodingFactory)
            {
            }

            protected override void DisposeInternal()
            {
                base.DisposeInternal();

                DisposeCount++;
            }
        }
    }
}