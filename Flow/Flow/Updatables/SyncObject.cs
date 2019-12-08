using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Flow.Interfaces;
using Flow.Updaters;

namespace Flow.Updatables
{
    public class SyncObject : Updatable
    {
        protected readonly object @lock = new object();
        public event SyncUpdateDelegate OnSyncUpdate;
        public bool IsLocked { get; private set; }
        protected override void InternalUpdate()
        {
            if (!IsLocked && Monitor.TryEnter(@lock))
            {
                IsLocked = true;
                try
                {
                    SyncUpdate();
                    IsLocked = false;
                    Monitor.Exit(@lock);
                }
                catch (Exception ex)
                {
                    IsLocked = false;
                    Monitor.Exit(@lock);
                    Handle(ex);
                }
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
            while (IsLocked)
            {
                Thread.Sleep(1);
                if (!IsLocked) break;
                if (stopwatch.Elasped >= timeout)
                {
                    stopwatch.Stop();
                    return false;
                }
            }
            stopwatch.Stop();
            return true;
        }
        protected virtual void SyncUpdate() => OnSyncUpdate?.Invoke(this);

        public SyncObject() { }
        public SyncObject(IUpdater updater) : base(updater) { }

        public delegate void SyncUpdateDelegate(object sender);
    }

    public interface ISyncObject
    {
        bool IsLocked { get; }
    }
}
