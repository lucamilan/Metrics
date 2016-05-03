using System;
using MiniMetrics.Extensions;

namespace MiniMetrics
{
    public class GraphiteFormatter
    {
        public String Format(String key, Object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (key.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(key));

            if (!value.IsNumber())
                throw new InvalidCastException($"value has wrong type {value?.GetType()}");

            return $"{Sanitize(key)} {value} { DateTimeExtensions.ToUnixTimestamp() }{Environment.NewLine}";
        }

        private static String Sanitize(String key)
        {
            return key.Replace("-", String.Empty)
                      .Trim()
                      .ToLowerInvariant();
        }
    }
}