using System;
using MiniMetrics.Extensions;
using MiniMetrics.Net;

namespace MiniMetrics
{
    public class Metrics
    {
        private static IMetricsClient MetricsClient = NullMetricsClient.Instance;

        private static readonly Object Sync = new Object();

        public static void StartFromConfig()
        {
            Start(MetricsOptions.CreateFromConfig());
        }

        public static void Start(MetricsOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            lock (Sync)
            {
                if (!(MetricsClient is NullMetricsClient))
                    return;

                StopInternal();
                TcpMetricsClient.StartAsync(OutbountChannel.From(options.HostName, options.Port).Build())
                                .ContinueWith(_ => MetricsClient = _.Result)
                                .Wait();
            }
        }

        public static void StartAutoRecoverable(MetricsOptions options, TimeSpan recoverySlice)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            lock (Sync)
            {
                if (!(MetricsClient is NullMetricsClient))
                    return;

                StopInternal();
                TcpMetricsClient.StartAsync(OutbountChannel.From(options.HostName, options.Port).BuildAutoRecoverable(recoverySlice))
                                .ContinueWith(_ => MetricsClient = _.Result)
                                .Wait();
            }
        }

        public static void Stop()
        {
            lock (Sync)
            {
                if (MetricsClient is NullMetricsClient)
                    return;

                StopInternal();
            }
        }

        public static void Report(String key, Single value)
        {
            MetricsClient.Report(key, value);
        }

        public static void Report(String key, Double value)
        {
            MetricsClient.Report(key, value);
        }

        public static void Report(String key, Int16 value)
        {
            MetricsClient.Report(key, value);
        }

        public static void Report(String key, Int32 value)
        {
            MetricsClient.Report(key, value);
        }

        public static void Report(String key, Int64 value)
        {
            MetricsClient.Report(key, value);
        }

        public static IDisposable ReportTimer(String key, Func<IStopwatch> stopWatchFactory = null)
        {
            return MetricsClient.ReportTimer(key, stopWatchFactory ?? SimpleStopwatch.StartNew);
        }

        private static void StopInternal()
        {
            MetricsClient.Dispose();
            MetricsClient = NullMetricsClient.Instance;
        }
    }
}