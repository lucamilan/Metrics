using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMetrics
{
    public static class SystemClock
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static Func<DateTime> Now = () => DateTime.Now;

        private static long ToUnixTimestamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        public static long ToUnixTimestamp()
        {
            return ToUnixTimestamp(Now());
        }
    }
}