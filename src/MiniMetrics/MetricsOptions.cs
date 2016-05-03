using System;
using System.Collections.Specialized;
using System.Configuration;

namespace MiniMetrics
{
    public class MetricsOptions
    {
        public const int GraphiteDefaultServerPort = 2003;
        private Func<IMetricsClient> metricsClient;
        private Func<IStopwatch> _stopwatch;
        private Func<string, string> _keyBuilder;

        public MetricsOptions()
        {
            Port = GraphiteDefaultServerPort;
        }

        public static MetricsOptions CreateFrom(NameValueCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var options = new MetricsOptions();
            options.HostName = collection["metrics:hostname"];
            options.Port = TryParsePort(collection["metrics:port"]);
            options.Prefix = collection["metrics:prefix"];

            return options;
        }

        public static MetricsOptions CreateFromConfig()
        {
            return CreateFrom(ConfigurationManager.AppSettings);
        }

        public string HostName { get; set; }

        public int Port { get; set; }

        public string Prefix { get; set; }

        public Func<IStopwatch> Stopwatch
        {
            get
            {
                return _stopwatch ?? new Func<IStopwatch>(() => SimpleStopwatch.StartNew());
            }
            set
            {
                _stopwatch = value;
            }
        }

        public Func<string, string> KeyBuilder
        {
            get
            {
                return _keyBuilder ?? new Func<string, string>(k => k);
            }
            set
            {
                _keyBuilder = value;
            }
        }

        public Func<IMetricsClient> MetricsClient
        {
            get
            {
                return metricsClient ?? new Func<IMetricsClient>(() => TcpMetricsClient.CreateFrom(HostName,Port));
            }
            set
            {
                metricsClient = value;
            }
        }

        private static int TryParsePort(string value)
        {
            int port;

            if (int.TryParse(value, out port))
            {
                return port;
            }

            return GraphiteDefaultServerPort;
        }
    }
}