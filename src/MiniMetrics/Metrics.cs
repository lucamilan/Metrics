﻿using System;
using MiniMetrics.Extensions;

namespace MiniMetrics
{
    // TODO: sync
    public class Metrics
    {
        private static IMetricsClient MetricsClient = new NullMetricsClient();
        private static MetricsOptions Options;

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
                Options = options;
                Stop();

                var client = options.MetricsClient();

                if (client == null)
                    throw new InvalidOperationException("metrics client is null");

                MetricsClient = client;
            }
        }

        public static void Stop()
        {
            lock (Sync)
                MetricsClient.Dispose();
        }

        public static void Report<TValue>(String key, TValue value)
        {
            MetricsClient.Report(Options.KeyBuilder(key), value);
        }

        public static IDisposable ReportTimer(String key)
        {
            return MetricsClient.ReportTimer(Options.KeyBuilder(key), Options.Stopwatch);
        }
    }
}