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
        }

        [Fact]
        public void CreateFromCollection()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["metrics:hostname"] = "localhost";
            collection["metrics:port"] = "8253";
            collection["metrics:prefix"] = "test";

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

            _sut.KeyBuilder = (k) =>
            {
                return $"test.{Environment.MachineName}.{k}";
            };

            string key = _sut.KeyBuilder.Invoke("component.mystats");

            Assert.Equal(expected, key);
        }

        [Fact]
        public void OverrideStopwatch()
        {
            const long expected = 0;

            _sut.Stopwatch = () => new FakeStopwatch();

            long elapsedMilliseconds = _sut.Stopwatch.Invoke().ElapsedMilliseconds;

            Assert.Equal(expected, elapsedMilliseconds);
        }
    }

    internal class FakeStopwatch : IStopwatch
    {
        public long ElapsedMilliseconds
        {
            get
            {
                return 0L;
            }
        }

        public long ElapsedTicks
        {
            get
            {
                return 0L;
            }
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}