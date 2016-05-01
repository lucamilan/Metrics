using Moq;
using System;
using Xunit;

namespace MiniMetrics.Tests
{
    public class MetricsTests : IDisposable
    {
        private Mock<IMetricsClient> _mockClient;
        private Mock<IStopwatch> _mockStopwatch;

        public MetricsTests()
        {
            var option = new MetricsOptions();

            _mockClient = new Mock<IMetricsClient>();
            _mockStopwatch = new Mock<IStopwatch>();

            option.Stopwatch = () => _mockStopwatch.Object;
            option.MetricsClient = () => _mockClient.Object;

            Metrics.Start(option);
        }

        [Fact]
        public void ReportIntValue()
        {
            const string key = "test";
            const int value = 100;

            string expected = new GraphiteFormatter().Format(key, value);

            Metrics.Report(key, value);

            _mockClient.Verify(t => t.Send(expected), Times.Once);
        }

        [Fact]
        public void ReportLongValue()
        {
            const string key = "test";
            const long value = 4294967296;

            string expected = new GraphiteFormatter().Format(key, value);

            Metrics.Report(key, value);

            _mockClient.Verify(t => t.Send(expected), Times.Once);
        }

        [Fact]
        public void ReportTimer()
        {
            const string key = "test";
            const long value = 4294967296;

            string expected = new GraphiteFormatter().Format(key, value);

            _mockStopwatch.SetupGet(t => t.ElapsedMilliseconds).Returns(value);

            using (Metrics.ReportTimer(key))
            {
                // do something
            }

            _mockClient.Verify(t => t.Send(expected), Times.Once);
        }

        public void Dispose()
        {
            Metrics.Stop();
        }
    }
}