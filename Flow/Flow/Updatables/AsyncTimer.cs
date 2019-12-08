using Flow.Interfaces;
using Flow.Updaters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Flow.Updatables
{
    public class AsyncTimer : AsyncObject, IAsyncTimer
    {
        private bool isLocked = false;
        protected readonly object @lock = new object();
        public event TickDelegate OnTick;
        public Stopwatch.Stopwatch Stopwatch { get; } = new Stopwatch.Stopwatch();
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan LastTick { get; set; } = TimeSpan.Zero;
        public TimeSpan LastError { get; set; } = TimeSpan.Zero;
        public bool IsTolerant { get; set; } = false;
        public bool ErrorCorrection { get; set; } = false;
        public TimeSpan NextTick
        {
            get => LastTick + Delay;
            set => LastTick = value - Delay;
        }

        public void TickNow() => NextTick = TimeSpan.Zero;
        public override void Start()
        {
            lock (actionLock)
            {
                base.Start();
                if (IsRunning) Stopwatch.Start();
            }
        }
        public override void Stop()
        {
            lock (actionLock)
            {
                base.Stop();
                if (!IsRunning) Stopwatch.Stop();
            }
        }
        public override void Reset()
        {
            lock (actionLock)
            {
                base.Reset();
                Stopwatch.Reset();
                LastTick = TimeSpan.Zero;
                LastError = TimeSpan.Zero;
            }
        }
        protected override void AsyncUpdate()
        {
            base.AsyncUpdate();
            var tick = false;
            TimeSpan time;
            if (!isLocked && Monitor.TryEnter(@lock))
            {
                isLocked = true;
                time = LastTick + Delay;
                var elasped = Stopwatch.Elasped;
                if (time - (ErrorCorrection ? LastError : TimeSpan.Zero) <= elasped)
                {
                    if (ErrorCorrection) LastError = LastError + elasped - time;
                    if (IsTolerant) LastTick = elasped; else LastTick = time;
                    tick = true;
                }
                isLocked = false;
                Monitor.Exit(@lock);
            }
            if (tick)
                try
                {
                    Tick(time);
                }
                catch (Exception ex)
                {
                    Handle(ex);
                }
        }

        protected virtual void Tick(TimeSpan time) => OnTick?.Invoke(this, time);

        public AsyncTimer() { }
        public AsyncTimer(IUpdater updater) : base(updater) { }

        public delegate void TickDelegate(object sender, TimeSpan time);
    }

    public interface IAsyncTimer : ITimer { }
}
