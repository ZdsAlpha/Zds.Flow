using Flow.Collections;
using Flow.Interfaces;
using Flow.Updaters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Updatables
{
    public class SyncSystem : SyncObject, ISyncSystem
    {
        private readonly SafeList<IUpdatable> targets = new SafeList<IUpdatable>();
        public IUpdatable[] Targets => targets.Elements;

        protected override void SyncUpdate()
        {
            base.SyncUpdate();
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

        protected void Add(IUpdatable obj)
        {
            if (IsDisposed || obj == null) return;
            if (obj.IsDisposed) return;
            lock (actionLock)
            {
                if (!((IUpdater)this).Contains(obj)) targets.Add(obj);
                if (obj.Updater != this) obj.Updater = this;
            }
        }
        protected void Remove(IUpdatable obj)
        {
            if (IsDisposed || obj == null) return;
            lock (actionLock)
            {
                targets.Remove(obj);
                if (obj.Updater == this) obj.Updater = null;
            }
        }
        protected void Clear()
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
        protected bool Contains(IUpdatable obj)
        {
            if (IsDisposed || obj == null) return false;
            lock (actionLock)
                return targets.Contains(obj);
        }

        protected void StartAll()
        {
            foreach (var target in Targets)
                target.Start();
            Start();
        }
        protected void StopAll()
        {
            Stop();
            foreach (var target in Targets)
                target.Stop();
        }

        public override void Dispose()
        {
            lock (actionLock)
            {
                Clear();
                base.Dispose();
            }
        }

        void IUpdater.Add(IUpdatable obj) => Add(obj);
        void IUpdater.Remove(IUpdatable obj) => Remove(obj);
        void IUpdater.Clear() => Clear();
        bool IUpdater.Contains(IUpdatable obj) => Contains(obj);

        public SyncSystem() { }
        public SyncSystem(IUpdater updater) : base(updater) { }
    }

    public interface ISyncSystem : ISystem { }
}
