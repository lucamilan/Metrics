using System;

namespace MiniMetrics
{
    public class Metrics
    {
        private static IMetricsClient MetricsClient = new NullMetricsClient();
        private static MetricsOptions Options;

        public static void StartFromConfig()
        {
            var options = MetricsOptions.CreateFromConfig();

            Start(options);
        }

        public static void Start(MetricsOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;

            Stop();

            var metricsClient = options.MetricsClient();

            if (metricsClient == null)
            {
                throw new InvalidOperationException("MetricsClient is null");
            }

            MetricsClient = metricsClient;
        }

        public static void Stop()
        {
            MetricsClient.Dispose();
        }

        public static void Report(string key, long value)
        {
            PrepareAndSend(key, value);
        }

        public static void Report(string key, int value)
        {
            PrepareAndSend(key, value);
        }

        public static IDisposable ReportTimer(string key)
        {
            return new Timer(key, Options.Stopwatch());
        }

        private static void PrepareAndSend(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var namespacedKey = Options.KeyBuilder(key);

            var message = new GraphiteFormatter().Format(namespacedKey, value);

            MetricsClient.Send(message);
        }

        private class Timer : IDisposable
        {
            private readonly string _key;
            private readonly IStopwatch _stopWatch;
            private bool _disposed;

            public Timer(string key, IStopwatch stopWatch)
            {
                _key = key;
                _stopWatch = stopWatch;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _stopWatch.Stop();
                    Report(_key, _stopWatch.ElapsedMilliseconds);
                }
            }
        }
    }
}