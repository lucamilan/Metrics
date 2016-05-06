using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MiniMetrics.Formatting;
using MiniMetrics.Net;
using Moq;
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
            const String key = "test";
            const Int32 value = 2;
            const String expected = "result";

            var @event = new ManualResetEvent(false);
            String result = null;

            var formatter = new Mock<IFormatter>();
            formatter.Setup(_ => _.Format(key, value)).Returns(expected);
            TcpMetricsClient.StartAsync(OutbountChannel.From(IPAddress.Loopback,
                                                             MetricsOptions.GraphiteDefaultServerPort)
                                                       .BuildAutoRecoverable(),
                                        formatter.Object)
                            .ContinueWith(_ =>
                                          {
                                              _.Result.OnMessageSent += (sender, args) =>
                                                                        {
                                                                            result = args.Message;
                                                                            @event.Set();
                                                                        };
                                              _.Result.Send(key, value);
                                          });

            if (!@event.WaitOne(TimeSpan.FromSeconds(10d)))
                throw new Exception("timed out");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void SendingAfterDisposing()
        {
            var wrapper = TcpMetricsClientWrapper.Stub();
            wrapper.Send("key_1", 1);
            wrapper.Dispose();
            Assert.Throws<ObjectDisposedException>(() => wrapper.Send("key_2", 2));
            wrapper.Dispose();
            Assert.Equal(1, wrapper.DisposeCount);
        }

        public void Dispose()
        {
            _listener.Stop();
        }
    }
}