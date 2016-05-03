using System;

namespace MiniMetrics
{
    public interface IMetricsClient : IDisposable
    {
        event EventHandler<MessageSentEventArgs> OnMessageSent;

        void Send(String message);
    }

    public class NullMetricsClient : IMetricsClient
    {
        public event EventHandler<MessageSentEventArgs> OnMessageSent;

        public void Dispose()
        {
        }

        public void Send(String message)
        {
        }
    }
}