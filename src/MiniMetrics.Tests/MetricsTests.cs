using System;
using MiniMetrics.Extensions;
using Moq;
using Xunit;

namespace MiniMetrics.Tests
{
    public class MetricsTests
    {
        [Fact]
        public void ReportLongValue()
        {
            const String expected = "result";
            const String key = "test";
            const Int64 value = 4294967296;

            using (var client = TcpMetricsClientWrapper.Stub(formatter: StubFormatter(key, value, expected).Object))
            {
                client.Send(key, value);
                Assert.Equal(expected, client.Dequeue());
            }
        }

        [Fact]
        public void ReportTimer()
        {
            const String key = "test";
            const Int64 value = 4294967296L;
            var expected = key + value;

            var _stopwatch = new Mock<IStopwatch>();
            _stopwatch.SetupGet(_ => _.ElapsedMilliseconds).Returns(value);

            using (var client = TcpMetricsClientWrapper.Stub(formatter: StubFormatter(key, value, expected).Object))
            {
                using (client.ReportTimer(key, () => _stopwatch.Object)) { }

                Assert.Equal(expected, client.Dequeue());
            }
        }

        private static Mock<IFormatter> StubFormatter(String key, Int64 value, String expected)
        {
            var formatter = new Mock<IFormatter>();
            formatter.Setup(_ => _.Format(key, value)).Returns(expected);
            return formatter;
        }
    }
}