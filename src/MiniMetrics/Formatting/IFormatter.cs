using System;

namespace MiniMetrics.Formatting
{
    public interface IFormatter
    {
        String Format(String key, Int16 value);

        String Format(String key, Int32 value);

        String Format(String key, Int64 value);
    }
}