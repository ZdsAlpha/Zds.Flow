using Flow.Collections;
using Flow.Interfaces;
using Flow.Updatables;
using Flow.Updaters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Actors
{
    public class AsyncActor<T> : AsyncObject, IAsyncSink<T>
    {
        private readonly IQueue<T> queue;
        public event ActorDelegate<T> OnAction;

        public override void Reset()
        {
            base.Reset();
            Sink<T>.Clear(queue);
        }

        protected override void AsyncUpdate()
        {
            base.AsyncUpdate();
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

        public AsyncActor() => queue = new DynamicArray<T>(1024 * 1024);
        public AsyncActor(IQueue<T> queue) => this.queue = queue;
        public AsyncActor(IUpdater updater) : base(updater) => queue = new DynamicArray<T>(1024 * 1024);
        public AsyncActor(IUpdater updater, IQueue<T> queue) : base(updater) => this.queue = queue;

        public static AsyncActor<T> CreateSink(SinkDelegate<T> OnSink, IUpdater updater, IQueue<T> queue)
        {
            var sink = new AsyncActor<T>(updater, queue);
            sink.OnAction += (s) => {
                T input = default;
                if (s.Receive(ref input))
                    OnSink(input);
            };
            return sink;
        }
        public static AsyncActor<T> CreateSink(SinkDelegate<T> OnSink, IUpdater updater)
        {
            var sink = new AsyncActor<T>(updater);
            sink.OnAction += (s) => {
                T input = default;
                if (s.Receive(ref input))
                    OnSink(input);
            };
            return sink;
        }
        public static AsyncActor<T> CreateSink(SinkDelegate<T> OnSink)
        {
            var sink = new AsyncActor<T>();
            sink.OnAction += (s) => {
                T input = default;
                if (s.Receive(ref input))
                    OnSink(input);
            };
            return sink;
        }
    }

    public interface IAsyncSink<T> : IAsyncObject, IUpdatableSink<T> { }
}
