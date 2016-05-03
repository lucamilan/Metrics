using System;
using System.Diagnostics;

namespace MiniMetrics
{
    public interface IStopwatch
    {
        Int64 ElapsedTicks { get; }

        Int64 ElapsedMilliseconds { get; }

        void Start();

        void Stop();
    }

    public class SimpleStopwatch : IStopwatch
    {
        private readonly Stopwatch _watch;

        public SimpleStopwatch(Stopwatch watch)
        {
            if (watch == null)
                throw new ArgumentNullException(nameof(watch));

            _watch = watch;
        }

        public Int64 ElapsedTicks => _watch.ElapsedTicks;

        public Int64 ElapsedMilliseconds => _watch.ElapsedMilliseconds;

        public static SimpleStopwatch StartNew()
        {
            return new SimpleStopwatch(Stopwatch.StartNew());
        }

        public void Start()
        {
            _watch.Start();
        }

        public void Stop()
        {
            _watch.Stop();
        }
    }
}