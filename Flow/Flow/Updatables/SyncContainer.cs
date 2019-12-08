using Flow.Interfaces;
using Flow.Updaters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Updatables
{
    public class SyncContainer : SyncSystem, ISyncContainer
    {
        public new void Add(IUpdatable obj) => base.Add(obj);
        public new void Remove(IUpdatable obj) => base.Remove(obj);
        public new void Clear() => base.Clear();
        public new bool Contains(IUpdatable obj) => base.Contains(obj);

        public new void StartAll() => base.StartAll();
        public new void StopAll() => base.StopAll();

        public SyncContainer() { }
        public SyncContainer(IUpdater updater) : base(updater) { }
    }

    public interface ISyncContainer : IContainer, ISyncSystem { }
}
