using System;

namespace MiniMetrics
{
    public class MessageSentEventArgs : EventArgs
    {
        internal MessageSentEventArgs(String message)
        {
            Message = message;
        }

        public String Message { get; }
    }
}