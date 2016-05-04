using System;

namespace MiniMetrics
{
    public interface IMetricsClient : IDisposable
    {
        event EventHandler<MessageSentEventArgs> OnMessageSent;

        void Send<TValue>(String key, TValue value);
    }
}