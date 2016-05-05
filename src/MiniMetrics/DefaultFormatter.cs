using System;
using MiniMetrics.Extensions;

namespace MiniMetrics
{
    public class DefaultFormatter : IFormatter
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

        public String Format<TValue>(String key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (key.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(key));

            if (!value.IsNumber())
                throw new NotSupportedException($"type '{value?.GetType().Name}' is not supported");

            return $"{Sanitize(_keyBuilder(key))} {value} { DateTimeExtensions.ToUnixTimestamp() }{Environment.NewLine}";
        }

        private static String Sanitize(String key)
        {
            return key.Replace("-", String.Empty)
                      .Trim()
                      .ToLowerInvariant(); // TODO
        }
    }

    public interface IFormatter
    {
        String Format<TValue>(String key, TValue value);
    }
}