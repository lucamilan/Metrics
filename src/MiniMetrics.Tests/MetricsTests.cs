using System;
using Moq;
using Xunit;

namespace MiniMetrics.Tests
{
    public class MetricsTests : IDisposable
    {
        private readonly Mock<IMetricsClient> _client;
        private readonly Mock<IStopwatch> _stopwatch;

        public MetricsTests()
        {
            var option = new MetricsOptions();

            _client = new Mock<IMetricsClient>();
            _stopwatch = new Mock<IStopwatch>();

            option.Stopwatch = () => _stopwatch.Object;
            option.MetricsClient = () => _client.Object;

            Metrics.Start(option);
        }

        [Fact]
        public void ReportIntValue()
        {
            const String key = "test";
            const Int32 value = 100;

            var expected = new GraphiteFormatter().Format(key, value);

            Metrics.Report(key, value);

            _client.Verify(_ => _.Send(expected), Times.Once);
        }

        [Fact]
        public void ReportLongValue()
        {
            const String key = "test";
            const Int64 value = 4294967296;

            var expected = new GraphiteFormatter().Format(key, value);

            Metrics.Report(key, value);

            _client.Verify(_ => _.Send(expected), Times.Once);
        }

        [Fact]
        public void ReportTimer()
        {
            const String key = "test";
            const Int64 value = 4294967296;

            var expected = new GraphiteFormatter().Format(key, value);

            _stopwatch.SetupGet(_ => _.ElapsedMilliseconds).Returns(value);

            using (Metrics.ReportTimer(key))
            {
                // do something
            }

            _client.Verify(_ => _.Send(expected), Times.Once);
        }

        public void Dispose()
        {
            Metrics.Stop();
        }
    }
}