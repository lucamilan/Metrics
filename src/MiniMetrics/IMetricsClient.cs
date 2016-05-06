using System;

namespace MiniMetrics
{
    public interface IMetricsClient : IDisposable
    {
        event EventHandler<MessageSentEventArgs> OnMessageSent;

        void Send(String key, Single value);

        void Send(String key, Double value);

        void Send(String key, Int16 value);

        void Send(String key, Int32 value);

        void Send(String key, Int64 value);
    }
}