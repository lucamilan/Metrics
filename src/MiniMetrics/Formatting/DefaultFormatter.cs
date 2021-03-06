using System;
using System.Globalization;
using MiniMetrics.Extensions;

namespace MiniMetrics.Formatting
{
    public class DefaultFormatter : IFormatter // TODO: should be an abstract class with key sanitization enforcing
    {
        private readonly Func<String, String> _keyBuilder;

        public DefaultFormatter()
            : this(null)
        {
        }

        public DefaultFormatter(Func<String, String> keyBuilder)
        {
            _keyBuilder = keyBuilder ?? (_ => _);
        }

        public String Format(String key, Single value)
        {
            return FormatInternal(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public String Format(String key, Double value)
        {
            return FormatInternal(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public String Format(String key, Int32 value)
        {
            return FormatInternal(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public String Format(String key, Int16 value)
        {
            return FormatInternal(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public String Format(String key, Int64 value)
        {
            return FormatInternal(key, value.ToString(CultureInfo.InvariantCulture));
        }

        private String FormatInternal(String key, String value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (key.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(key));

            return $"{SanitizeKey(_keyBuilder(key))} {value} { DateTimeExtensions.ToUnixTimestamp() }\n";
        }

        private static String SanitizeKey(String key)
        {
            return key.Replace("-", String.Empty)
                      .Trim()
                      .ToLowerInvariant();
        }
    }
}