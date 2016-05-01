using System;

namespace MiniMetrics
{
    public class GraphiteFormatter
    {
        public string Format(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!IsNumber(value))
            {
                throw new InvalidCastException($"Value has wrong type {value?.GetType()}");
            }

            return $"{Sanitize(key)} {value} { SystemClock.ToUnixTimestamp() }\n";
        }

        private static string Sanitize(string key)
        {
            return key.Replace("-", "")
                      .Trim()
                      .ToLowerInvariant();
        }

        private static bool IsNumber(object value)
        {
            return value is int || value is long;
        }
    }
}