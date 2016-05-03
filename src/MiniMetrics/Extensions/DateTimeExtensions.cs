using System;

namespace MiniMetrics.Extensions
{
    public static class DateTimeExtensions
    {
        public static Func<DateTime> Now = () => DateTime.UtcNow;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static Int64 ToUnixTimestamp()
        {
            return Now().ToUnixTimestamp();
        }

        private static Int64 ToUnixTimestamp(this DateTime dateTime)
        {
            return (Int64)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }
    }
}