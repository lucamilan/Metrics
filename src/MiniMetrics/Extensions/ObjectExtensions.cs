using System;

namespace MiniMetrics.Extensions
{
    public static class ObjectExtensions
    {
        public static Boolean IsNumber(this Object value)
        {
            // TODO: what about floating point numbers?
            return value is Int16 || value is Int32 || value is Int64;
        }
    }
}