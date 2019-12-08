using System;
using System.Collections.Generic;
using System.Text;
using Flow.Interfaces;
using System.Threading;
using Flow.Updaters;

namespace Flow.Updatables
{
    public abstract class Updatable : IUpdatable
    {
        private IUpdater updater;
        protected readonly object actionLock = new object();
        public bool IsDisposed { get; private set; } = false;
        public bool IsRunning { get; private set; } = false;
        public IUpdater Updater {
            get => updater;
            set
            {
                if (updater == value) return;
                lock (actionLock)
                {
                    if (updater == null && value != null)
                    {
                        if (IsDisposed) return;
                        updater = value;
                        value.Add(this);
                        if (!value.Contains(this))
                            updater = null;
                    }
                    else if (updater != null && value == null)
                    {
                        var _updater = updater;
                        updater = null;
                        _updater.Remove(this);
                        if (_updater.Contains(this))
                            updater = _updater;
                    }
                    else if (updater != null && value != null)
                    {
                        Updater = null;
                        if (updater != null) return;
                        Updater = value;
                    }
                }
            }
        }
        public event OnExceptionDelegate OnException;
        public event OnUpdateDelegate PreUpdate;
        public event OnUpdateDelegate PostUpdate;

        public void Update()
        {
            if (!IsDisposed && IsRunning)
            {
                try
                {
                    PreUpdate?.Invoke(this);
                    InternalUpdate();
                    PostUpdate?.Invoke(this);
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
            Updater?.Throw(this, ex);
            if (ex.GetType() == typeof(ThreadAbortException)) Thread.ResetAbort();
        }
        void IThrowsException.Throw(object sender, Exception ex) => OnException?.Invoke(sender, ex);
        public virtual void Dispose()
        {
            lock (actionLock)
            {
                Updater = null;
                Reset();
                IsDisposed = true;
            }
        }

        public abstract void Reset();
        public abstract bool Wait(TimeSpan timeout);
        public void Wait() => Wait(TimeSpan.MaxValue);
        protected abstract void InternalUpdate();

        public Updatable() => Updater = Updaters.Updater.DefaultUpdater;
        public Updatable(IUpdater updater) => Updater = updater;

        public delegate void OnUpdateDelegate(object sender);
    }

    public interface IUpdatable : IStartStopable, IThrowsException, IDisposable
    {
        bool IsDisposed { get; }
        IUpdater Updater { get; set; }
        void Update();
        bool Wait(TimeSpan timeout);
    }
}
