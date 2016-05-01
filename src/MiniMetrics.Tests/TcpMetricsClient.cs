using System;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace MiniMetrics.Tests
{
    public class TcpMetricsClient : IDisposable
    {
        private readonly TcpListener _tcpListener;
        private readonly MiniMetrics.TcpMetricsClient _tcpMetricsClient;

        public TcpMetricsClient()
        {
            _tcpListener = new TcpListener(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort);
            _tcpListener.Start();

            _tcpMetricsClient = new MiniMetrics.TcpMetricsClient(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort);
        }

        [Fact]
        public void SendMessage()
        {
            _tcpMetricsClient.Send("test");
        }

        [Fact]
        public void ThrowOperationCanceledExceptionAfterDisposing()
        {
            _tcpMetricsClient.Dispose();

            Assert.Throws<OperationCanceledException>(() => _tcpMetricsClient.Send("test"));
        }

        public void Dispose()
        {
            _tcpMetricsClient.Dispose();
            _tcpListener.Stop();
        }
    }
}