using System;
using System.Diagnostics;

namespace MiniMetrics
{
    public interface IStopwatch
    {
        long ElapsedTicks { get; }

        long ElapsedMilliseconds { get; }

        void Start();

        void Stop();
    }

    public class SimpleStopwatch : IStopwatch
    {
        private readonly Stopwatch _watch;

        public SimpleStopwatch(Stopwatch watch)
        {
            if (watch == null)
            {
                throw new ArgumentNullException(nameof(watch));
            }

            _watch = watch;
        }

        public long ElapsedTicks
        {
            get { return _watch.ElapsedTicks; }
        }

        public long ElapsedMilliseconds
        {
            get { return _watch.ElapsedMilliseconds; }
        }

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