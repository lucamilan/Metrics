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
                                              Func<IStopwatch> stopwatch = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return new Timer(key, (stopwatch ?? SimpleStopwatch.StartNew)(), client.Send);
        }

        private class Timer : IDisposable
        {
            private readonly String _key;
            private readonly IStopwatch _stopWatch;
            private readonly Action<String, Int64> _action;

            private Int32 _disposed;

            public Timer(String key, IStopwatch stopWatch, Action<String, Int64> action)
            {
                _key = key;
                _stopWatch = stopWatch;
                _action = action;
            }

            public void Dispose()
            {
                var i = Interlocked.CompareExchange(ref _disposed, 1, 0);

                if (i == 1)
                    return;

                _stopWatch.Stop();
                _action(_key, _stopWatch.ElapsedMilliseconds);
            }
        }
    }
}