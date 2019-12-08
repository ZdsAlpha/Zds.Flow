using Flow.Updaters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Flow.Updatables
{
    public class AsyncObject : Updatable, IAsyncObject
    {
        private int activeThreads = 0;
        public event AsyncUpdateDelegate OnAsyncUpdate;
        public int ActiveThreads { get => activeThreads; private set => activeThreads = value; }
        public int MaxThreads { get; set; } = int.MaxValue;

        protected override void InternalUpdate()
        {
            if (activeThreads < MaxThreads)
            {
                Interlocked.Increment(ref activeThreads);
                if (activeThreads <= MaxThreads)
                    try
                    {
                        AsyncUpdate();
                    }
                    catch (Exception ex)
                    {
                        Handle(ex);
                    }
                Interlocked.Decrement(ref activeThreads);
            }
        }
        public override void Reset()
        {
            lock (actionLock)
            {
                Stop();
                Wait();
            }
        }
        public override bool Wait(TimeSpan timeout)
        {
            Stopwatch.Stopwatch stopwatch = new Stopwatch.Stopwatch();
            stopwatch.Start();
            while (activeThreads != 0)
            {
                Thread.Sleep(1);
                if (activeThreads == 0) break;
                if (stopwatch.Elasped >= timeout)
                {
                    stopwatch.Stop();
                    return false;
                }
            }
            stopwatch.Stop();
            return true;
        }
        protected virtual void AsyncUpdate() => OnAsyncUpdate?.Invoke(this);

        public AsyncObject() { }
        public AsyncObject(IUpdater updater) : base(updater) { }

        public delegate void AsyncUpdateDelegate(object sender);
    }

    public interface IAsyncObject : IUpdatable {
        int ActiveThreads { get; }
        int MaxThreads { get; set; }
    }
}
