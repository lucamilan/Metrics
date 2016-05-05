using System;

namespace MiniMetrics
{
    public class NullMetricsClient : IMetricsClient
    {
        public event EventHandler<MessageSentEventArgs> OnMessageSent;

        public void Send(String key, Single value)
        {
        }

        public void Send(String key, Double value)
        {
        }

        public void Send(String key, Int16 value)
        {
        }

        public void Send(String key, Int32 value)
        {
        }

        public void Send(String key, Int64 value)
        {
        }

        public void Dispose()
        {
        }
    }
}