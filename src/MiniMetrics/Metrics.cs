using System;
using System.Threading;

namespace MiniMetrics
{
    public class Metrics
    {
        private static IMetricsClient MetricsClient = new NullMetricsClient();
        private static MetricsOptions Options;

        public static void StartFromConfig()
        {
            Start(MetricsOptions.CreateFromConfig());
        }

        public static void Start(MetricsOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Options = options;

            Stop();

            var client = options.MetricsClient();

            if (client == null)
                throw new InvalidOperationException("MetricsClient is null");

            MetricsClient = client;
        }

        public static void Stop()
        {
            MetricsClient.Dispose();
        }

        public static void Report(String key, Int64 value)
        {
            PrepareAndSend(key, value);
        }

        public static void Report(String key, Int32 value)
        {
            PrepareAndSend(key, value);
        }

        public static IDisposable ReportTimer(String key)
        {
            return new Timer(key, Options.Stopwatch());
        }

        private static void PrepareAndSend(String key, Object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var namespacedKey = Options.KeyBuilder(key);
            var message = new GraphiteFormatter().Format(namespacedKey, value);

            MetricsClient.Send(message);
        }

        private class Timer : IDisposable
        {
            private readonly String _key;
            private readonly IStopwatch _stopWatch;

            private Int32 _disposed;

            public Timer(String key, IStopwatch stopWatch)
            {
                _key = key;
                _stopWatch = stopWatch;
            }

            public void Dispose()
            {
                var i = Interlocked.CompareExchange(ref _disposed, 1, 0);

                if (i == 1)
                    return;

                _stopWatch.Stop();
                Report(_key, _stopWatch.ElapsedMilliseconds);
            }
        }
    }
}