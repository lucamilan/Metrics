using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
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

            MiniMetrics.TcpMetricsClient.StartAsync(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort)
                                        .ContinueWith(_ =>
                                                      {
                                                          _.Result.OnMessageSent += (sender, args) =>
                                                                                    {
                                                                                        result = message;
                                                                                        @event.Set();
                                                                                    };
                                                          _.Result.Send(message);
                                                      });

            @event.WaitOne(TimeSpan.FromSeconds(10d));
            Assert.Equal(message, result);
        }

        public void Dispose()
        {
            _listener.Stop();
        }
    }
}