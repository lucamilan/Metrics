using System;
using System.Threading;

namespace MiniMetrics.Extensions
{
    public static class TcpMetricsClientExtensions
    {
        public static void Report<TValue>(this IMetricsClient client,
                                          String key,
                                          TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            client.Send(key, value);
        }

        public static IDisposable ReportTimer(this IMetricsClient client,
                                              String key,
                                              Func<IStopwatch> stopwatchFactory = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return new Timer(key, (stopwatchFactory ?? SimpleStopwatch.StartNew)(), client.Send);
        }

        private class Timer : IDisposable
        {
            private readonly String _key;
            private readonly IStopwatch _stopwatchFactory;
            private readonly Action<String, Int64> _action;

            private Int32 _disposed;

            public Timer(String key, IStopwatch stopwatchFactory, Action<String, Int64> action)
            {
                _key = key;
                _stopwatchFactory = stopwatchFactory;
                _action = action;
            }

            public void Dispose()
            {
                var i = Interlocked.CompareExchange(ref _disposed, 1, 0);

                if (i == 1)
                    return;

                _stopwatchFactory.Stop();
                _action(_key, _stopwatchFactory.ElapsedMilliseconds);
            }
        }
    }
}