using System;
using System.Collections.Generic;
using System.Text;
using Flow.Updatables;
using Flow.Collections;
using Flow.Interfaces;
using System.Threading;

namespace Flow.Updaters
{
    public abstract class Updater : IUpdater
    {
        private readonly SafeList<IUpdatable> targets = new SafeList<IUpdatable>();
        protected readonly object actionLock = new object();
        public bool IsDisposed { get; private set; } = false;
        public bool IsRunning { get; private set; } = false;
        public IUpdatable[] Targets => targets.Elements;
        public event OnExceptionDelegate OnException;
        public abstract event ThreadEventDelegate ThreadCreated;
        public abstract event ThreadEventDelegate ThreadDestroyed;

        public virtual void Add(IUpdatable obj)
        {
            if (IsDisposed || obj == null) return;
            if (obj.IsDisposed) return;
            lock (actionLock)
            {
                if (!Contains(obj)) targets.Add(obj);
                if (obj.Updater != this) obj.Updater = this;
            }
        }
        public virtual void Remove(IUpdatable obj)
        {
            if (IsDisposed || obj == null) return;
            lock (actionLock)
            {
                targets.Remove(obj);
                if (obj.Updater == this) obj.Updater = null;
            }
        }
        public virtual void Clear()
        {
            if (IsDisposed) return;
            lock (actionLock)
            {
                var cache = targets.Elements;
                targets.Clear();
                foreach (var obj in cache)
                    if (obj.Updater == this) obj.Updater = null;
            }
        }
        public virtual bool Contains(IUpdatable obj)
        {
            if (IsDisposed) return false;
            lock (actionLock)
                return targets.Contains(obj);
        }
        protected void Update()
        {
            foreach (var target in targets.Elements)
            {
                if (IsDisposed || !IsRunning) break;
                try
                {
                    target.Update();
                }
                catch (Exception ex)
                {
                    Handle(ex);
                }
            }
        }
        public virtual void Start()
        {
            lock (actionLock)
                if (!IsDisposed) IsRunning = true;
        }
        public virtual void Stop()
        {
            lock (actionLock)
                IsRunning = false;
        }

        protected virtual void Handle(Exception ex)
        {
            ((IThrowsException)this).Throw(this, ex);
            if (ex.GetType() == typeof(ThreadAbortException)) Thread.ResetAbort();
        }
        void IThrowsException.Throw(object sender, Exception ex) => OnException?.Invoke(sender, ex);
        public virtual void Dispose()
        {
            IUpdatable[] targets;
            lock (actionLock)
            {
                targets = this.targets.Elements;
                Reset();
                this.targets.Clear();
                IsDisposed = true;
            }
            foreach (var target in targets)
                target.Dispose();
        }

        public abstract void Reset();
        public abstract bool Wait(TimeSpan timeout);
        public void Wait() => Wait(TimeSpan.MaxValue);

        public delegate void ThreadEventDelegate(object sender, Thread thread);

        public static IUpdater DefaultUpdater = new UpdaterX();

        static Updater() => DefaultUpdater.Start();
    }

    public interface IUpdater : IStartStopable, IThrowsException, IDisposable
    {
        bool IsDisposed { get; }
        IUpdatable[] Targets { get; }
        void Add(IUpdatable obj);
        void Remove(IUpdatable obj);
        bool Contains(IUpdatable obj);
        void Clear();
        bool Wait(TimeSpan timeout);
    }
}
