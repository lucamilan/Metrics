using System;
using MiniMetrics.Extensions;

namespace MiniMetrics
{
    public class DefaultFormatter : IFormatter
    {
        public String Format<TValue>(String key, TValue value)
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
                      .ToLowerInvariant(); // TODO
        }
    }

    public interface IFormatter
    {
        String Format<TValue>(String key, TValue value);
    }
}