using System;
using System.Collections.Specialized;
using Xunit;

namespace MiniMetrics.Tests
{
    public class MetricsOptionsTests
    {
        private MetricsOptions _sut;

        public MetricsOptionsTests()
        {
            _sut = new MetricsOptions();
        }

        [Fact]
        public void DefaultValues()
        {
            Assert.Equal(_sut.HostName, null);
            Assert.Equal(_sut.Port, MetricsOptions.GraphiteDefaultServerPort);
            Assert.IsType<NullMetricsClient>(_sut.MetricsClient());
        }

        [Fact]
        public void CreateFromCollection()
        {
            var collection = new NameValueCollection
                                 {
                                     ["metrics:hostname"] = "localhost",
                                     ["metrics:port"] = "8253",
                                     ["metrics:prefix"] = "test"
                                 };

            _sut = MetricsOptions.CreateFrom(collection);

            Assert.Equal(_sut.HostName, "localhost");
            Assert.Equal(_sut.Port, 8253);
        }

        [Fact]
        public void CreateFromConfig()
        {
            _sut = MetricsOptions.CreateFromConfig();

            Assert.Equal(_sut.HostName, "localhost");
            Assert.Equal(_sut.Port, 8253);
        }
    }

    internal class FakeStopwatch : IStopwatch
    {
        public Int64 ElapsedMilliseconds => 0L;

        public Int64 ElapsedTicks => 0L;

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}