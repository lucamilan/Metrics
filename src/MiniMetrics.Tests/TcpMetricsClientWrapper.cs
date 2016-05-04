using System;
using System.Text;
using MiniMetrics.Net;
using Moq;

namespace MiniMetrics.Tests
{
    internal class TcpMetricsClientWrapper : TcpMetricsClient
    {
        internal Int32 DisposeCount;

        private TcpMetricsClientWrapper(IOutbountChannel channel,
                                         TimeSpan breathTime,
                                         IFormatter formatter,
                                         Func<Encoding> encodingFactory)
            : base(channel, breathTime, formatter, encodingFactory)
        {
        }

        internal static TcpMetricsClientWrapper Stub(IOutbountChannel channel = null,
                                                     IFormatter formatter = null,
                                                     Func<Encoding> encodingFactory = null)
        {
            return new TcpMetricsClientWrapper(channel ?? new Mock<IOutbountChannel>().Object,
                                               TimeSpan.FromMilliseconds(-1d),
                                               formatter ?? new Mock<IFormatter>().Object,
                                               encodingFactory ?? (() => new Mock<Encoding>().Object));
        }

        internal String Dequeue()
        {
            String result;
            Queue.TryDequeue(out result);
            return result;
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();

            DisposeCount++;
        }
    }
}