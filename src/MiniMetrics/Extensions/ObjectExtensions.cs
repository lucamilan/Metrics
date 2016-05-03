using System;

namespace MiniMetrics.Extensions
{
    public static class ObjectExtensions
    {
        public static Boolean IsNumber(this Object value)
        {
            return value is Int32 || value is Int64;
        }
    }
}