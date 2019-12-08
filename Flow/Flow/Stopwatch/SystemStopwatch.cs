using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Stopwatch
{
    public class SystemStopwatch : IStopwatch
    {
        private readonly System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public bool IsRunning => stopwatch.IsRunning;
        public TimeSpan Elasped {
            get => stopwatch.Elapsed;
            set => throw new InvalidOperationException("You can assign this property to SystemStopwatch.");
        }
        public void Start() => stopwatch.Start();
        public void Stop() => stopwatch.Stop();
        public void Reset() => stopwatch.Reset();
    }
}
