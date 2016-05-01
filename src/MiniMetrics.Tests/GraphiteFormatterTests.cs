using System;
using Xunit;

namespace MiniMetrics.Tests
{
    public class GraphiteFormatterTests : IDisposable
    {
        private GraphiteFormatter _sut;

        public GraphiteFormatterTests()
        {
            SystemClock.Now = () => DateTime.Today;
            _sut = new GraphiteFormatter();
        }

        [Theory]
        [InlineData(100L)]
        [InlineData(100)]
        public void OnlyIntegerAndLongTypesAreSupported(object value)
        {
            string expected = $"test {value} { SystemClock.ToUnixTimestamp() }\n";

            string message = _sut.Format("test", value);

            Assert.Equal(expected, message);
        }

        [Theory]
        [InlineData(10D)]
        [InlineData(10F)]
        [InlineData(short.MaxValue)]
        [InlineData(uint.MinValue)]
        [InlineData(ulong.MinValue)]
        [InlineData(ushort.MinValue)]
        [InlineData("test")]
        [InlineData(null)]
        public void NonNumberTypesThrowException(Object value)
        {
            Assert.Throws<InvalidCastException>(() => _sut.Format("test", value));
        }

        public void Dispose()
        {
        }
    }
}