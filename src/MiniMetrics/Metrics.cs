using System;
using MiniMetrics.Extensions;
using MiniMetrics.Net;

namespace MiniMetrics
{
    public class Metrics
    {
        private static IMetricsClient MetricsClient = new NullMetricsClient();

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
                if (MetricsClient != null)
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
                if (MetricsClient == null)
                    return;

                StopInternal();
            }
        }

        public static void Report(String key, Single value)
        {
            lock (Sync)
            {
                ThrowOnNullClient();
                MetricsClient.Report(key, value);
            }
        }

        public static void Report(String key, Double value)
        {
            lock (Sync)
            {
                ThrowOnNullClient();
                MetricsClient.Report(key, value);
            }
        }

        public static void Report(String key, Int16 value)
        {
            lock (Sync)
            {
                ThrowOnNullClient();
                MetricsClient.Report(key, value);
            }
        }

        public static void Report(String key, Int32 value)
        {
            lock (Sync)
            {
                ThrowOnNullClient();
                MetricsClient.Report(key, value);
            }
        }

        public static void Report(String key, Int64 value)
        {
            lock (Sync)
            {
                ThrowOnNullClient();
                MetricsClient.Report(key, value);
            }
        }

        public static IDisposable ReportTimer(String key, Func<IStopwatch> stopWatchFactory = null)
        {
            lock (Sync)
            {
                ThrowOnNullClient();
                return MetricsClient.ReportTimer(key, stopWatchFactory ?? SimpleStopwatch.StartNew);
            }
        }

        private static void StopInternal()
        {
            MetricsClient?.Dispose();
            MetricsClient = null;
        }

        private static void ThrowOnNullClient()
        {
            if (MetricsClient == null)
                throw new InvalidOperationException("client has to be started by calling 'Start' method");
        }
    }
}