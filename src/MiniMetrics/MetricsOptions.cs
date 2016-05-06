using System;
using System.Collections.Specialized;
using System.Configuration;

namespace MiniMetrics
{
    public class MetricsOptions
    {
        public const Int32 GraphiteDefaultServerPort = 2003;

        private Func<IMetricsClient> _metricsClient;

        public MetricsOptions()
        {
            Port = GraphiteDefaultServerPort;
        }

        public String HostName { get; set; }

        public Int32 Port { get; set; }

        public Func<IMetricsClient> MetricsClient
        {
            get { return _metricsClient ?? (() => NullMetricsClient.Instance); }
            set { _metricsClient = value; }
        }

        public static MetricsOptions CreateFrom(NameValueCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return new MetricsOptions
                       {
                           HostName = collection["metrics:hostname"],
                           Port = TryParsePort(collection["metrics:port"])
                       };
        }

        public static MetricsOptions CreateFromConfig()
        {
            return CreateFromConfig(ConfigurationManager.AppSettings);
        }

        public static MetricsOptions CreateFromConfig(NameValueCollection settings)
        {
            return CreateFrom(settings);
        }

        private static Int32 TryParsePort(String value)
        {
            Int32 port;

            return Int32.TryParse(value, out port) ? port : GraphiteDefaultServerPort;
        }
    }
}