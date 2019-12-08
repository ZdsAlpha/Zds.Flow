using Flow.Interfaces;
using Flow.Updaters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Updatables
{
    public class AsyncContainer : AsyncSystem, IAsyncContainer
    {
        public new void Add(IUpdatable obj) => base.Add(obj);
        public new void Remove(IUpdatable obj) => base.Remove(obj);
        public new void Clear() => base.Clear();
        public new bool Contains(IUpdatable obj) => base.Contains(obj);

        public new void StartAll() => base.StartAll();
        public new void StopAll() => base.StopAll();

        public AsyncContainer() { }
        public AsyncContainer(IUpdater updater) : base(updater) { }
    }

    public interface IAsyncContainer : IContainer, IAsyncSystem { }
}
