using System;
using System.Collections.Generic;
using System.Text;
using Flow.Interfaces;

namespace Flow.Stopwatch
{
    public class Stopwatch : IStopwatch
    {
        private IStopwatch stopwatch = GlobalStopwatch;
        private TimeSpan time = TimeSpan.Zero;
        public bool IsRunning { get; private set; }
        public IStopwatch Reference
        {
            get => stopwatch;
            set
            {
                Stop();
                stopwatch = value;
                Start();
            }
        }
        public TimeSpan Elasped
        {
            get
            {
                if (IsRunning) return stopwatch.Elasped - time;
                else return time;
            }
            set
            {
                if (IsRunning) time = stopwatch.Elasped - value;
                else time = value;
            }
        }
        public void Start()
        {
            if (!IsRunning)
            {
                time = stopwatch.Elasped - time;
                IsRunning = true;
            }
        }
        public void Stop()
        {
            if (IsRunning)
            {
                time = Elasped;
                IsRunning = false;
            }
        }
        public void Reset()
        {
            Stop();
            time = TimeSpan.Zero;
        }
        public static readonly IStopwatch GlobalStopwatch = new SystemStopwatch();
        public Stopwatch() { }
        public Stopwatch(IStopwatch reference)
        {
            this.stopwatch = reference;
        }

        static Stopwatch() => GlobalStopwatch.Start();
    }

    public interface IStopwatch : IStartStopable
    {
        TimeSpan Elasped { get; set; }
    }
}
