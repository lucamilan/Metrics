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
            Assert.Equal(_sut.Prefix, null);
            Assert.Equal(_sut.Port, MetricsOptions.GraphiteDefaultServerPort);
            Assert.Equal(_sut.KeyBuilder.Invoke("test"), "test");
            Assert.IsType<NullMetricsClient>(_sut.MetricsClient());
            Assert.IsType<SimpleStopwatch>(_sut.Stopwatch());
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
            Assert.Equal(_sut.Prefix, "test");
            Assert.Equal(_sut.Port, 8253);
        }

        [Fact]
        public void CreateFromConfig()
        {
            _sut = MetricsOptions.CreateFromConfig();

            Assert.Equal(_sut.HostName, "localhost");
            Assert.Equal(_sut.Prefix, "test");
            Assert.Equal(_sut.Port, 8253);
        }

        [Fact]
        public void OverrideKeyBuilder()
        {
            var expected = $"test.{Environment.MachineName}.component.mystats";

            _sut.KeyBuilder = _ => $"test.{Environment.MachineName}.{_}";

            var key = _sut.KeyBuilder.Invoke("component.mystats");

            Assert.Equal(expected, key);
        }

        [Fact]
        public void OverrideStopwatch()
        {
            const Int64 expected = 0;

            _sut.Stopwatch = () => new FakeStopwatch();

            var elapsedMilliseconds = _sut.Stopwatch.Invoke().ElapsedMilliseconds;

            Assert.Equal(expected, elapsedMilliseconds);
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