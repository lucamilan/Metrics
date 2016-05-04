using System;

namespace MiniMetrics
{
    public class NullMetricsClient : IMetricsClient
    {
        public event EventHandler<MessageSentEventArgs> OnMessageSent;

        public void Send<TValue>(String key, TValue value)
        {
        }

        public void Dispose()
        {
        }
    }
}