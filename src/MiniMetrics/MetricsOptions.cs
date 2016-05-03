using System;
using System.Collections.Specialized;
using System.Configuration;

namespace MiniMetrics
{
    public class MetricsOptions
    {
        public const Int32 GraphiteDefaultServerPort = 2003;
        private Func<IMetricsClient> metricsClient;
        private Func<IStopwatch> _stopwatch;
        private Func<String, String> _keyBuilder;

        public MetricsOptions()
        {
            Port = GraphiteDefaultServerPort;
        }

        public String HostName { get; set; }

        public Int32 Port { get; set; }

        public String Prefix { get; set; }

        public Func<IStopwatch> Stopwatch
        {
            get { return _stopwatch ?? SimpleStopwatch.StartNew; }
            set { _stopwatch = value; }
        }

        public Func<String, String> KeyBuilder
        {
            get { return _keyBuilder ?? (_ => _); }
            set { _keyBuilder = value; }
        }

        public Func<IMetricsClient> MetricsClient
        {
            get { return metricsClient ?? (() => new NullMetricsClient()); }
            set { metricsClient = value; }
        }

        public static MetricsOptions CreateFrom(NameValueCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return new MetricsOptions
            {
                HostName = collection["metrics:hostname"],
                Port = TryParsePort(collection["metrics:port"]),
                Prefix = collection["metrics:prefix"]
            };
        }

        public static MetricsOptions CreateFromConfig()
        {
            return CreateFrom(ConfigurationManager.AppSettings);
        }

        private static Int32 TryParsePort(String value)
        {
            Int32 port;

            return Int32.TryParse(value, out port) ? port : GraphiteDefaultServerPort;
        }
    }
}