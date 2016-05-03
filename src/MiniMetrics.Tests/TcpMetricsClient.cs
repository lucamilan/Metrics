using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MiniMetrics.Net;
using Xunit;

namespace MiniMetrics.Tests
{
    public class TcpMetricsClient : IDisposable
    {
        private readonly TcpListener _listener;

        public TcpMetricsClient()
        {
            _listener = new TcpListener(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort);
            _listener.Start();
        }

        [Fact]
        public void SendMessage()
        {
            const String message = "test";

            var @event = new ManualResetEvent(false);
            var result = string.Empty;

            MiniMetrics.TcpMetricsClient.StartAsync(OutbountChannel.From(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort)
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

        public void Dispose()
        {
            _listener.Stop();
        }
    }
}