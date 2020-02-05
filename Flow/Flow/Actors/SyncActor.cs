using Flow.Collections;
using Flow.Interfaces;
using Flow.Updatables;
using Flow.Updaters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Actors
{
    public class SyncActor<T> : SyncObject, ISyncSink<T>
    {
        private readonly IQueue<T> queue;
        public event ActorDelegate<T> OnAction;

        public override void Reset()
        {
            base.Reset();
            Sink<T>.Clear(queue);
        }

        protected override void SyncUpdate()
        {
            base.SyncUpdate();
            Action();
        }
        protected virtual void Action() => OnAction?.Invoke(this);

        public bool Send(T obj) => Sink<T>.Send(queue, obj);
        public int Send(T[] array, int index, int length) => Sink<T>.Send(queue, array, index, length);
        public int Send(T[] array) => Sink<T>.Send(queue, array);

        protected bool Receive(ref T obj) => Sink<T>.Receive(queue, ref obj);
        protected int Receive(T[] array, int index, int length) => Sink<T>.Receive(queue, array, index, length);
        protected int Receive(T[] array) => Sink<T>.Receive(queue, array);

        bool IUpdatableSink<T>.Receive(ref T obj) => Receive(ref obj);
        int IUpdatableSink<T>.Receive(T[] array) => Receive(array);
        int IUpdatableSink<T>.Receive(T[] array, int index, int length) => Receive(array, index, length);

        public SyncActor() => queue = new DynamicArray<T>(1024 * 1024);
        public SyncActor(IQueue<T> queue) => this.queue = queue;
        public SyncActor(IUpdater updater) : base(updater) => queue = new DynamicArray<T>(1024 * 1024);
        public SyncActor(IUpdater updater, IQueue<T> queue) : base(updater) => this.queue = queue;

        public static SyncActor<T> CreateSink(SinkDelegate<T> OnSink, IUpdater updater, IQueue<T> queue)
        {
            var sink = new SyncActor<T>(updater, queue);
            sink.OnAction += (s) => {
                T input = default;
                if (s.Receive(ref input))
                    OnSink(input);
            };
            return sink;
        }
        public static SyncActor<T> CreateSink(SinkDelegate<T> OnSink, IUpdater updater)
        {
            var sink = new SyncActor<T>(updater);
            sink.OnAction += (s) => {
                T input = default;
                if (s.Receive(ref input))
                    OnSink(input);
            };
            return sink;
        }
        public static SyncActor<T> CreateSink(SinkDelegate<T> OnSink)
        {
            var sink = new SyncActor<T>();
            sink.OnAction += (s) => {
                T input = default;
                if (s.Receive(ref input))
                    OnSink(input);
            };
            return sink;
        }
    }

    public interface ISyncSink<T> : ISyncObject, IUpdatableSink<T> { }
}
