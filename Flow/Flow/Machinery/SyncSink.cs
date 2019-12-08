﻿using Flow.Collections;
using Flow.Interfaces;
using Flow.Updatables;
using Flow.Updaters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Machinery
{
    public class SyncSink<T> : SyncObject, ISyncSink<T>
    {
        private readonly IQueue<T> queue;
        public event SinkDelegate<T> OnSink;

        public override void Reset()
        {
            base.Reset();
            Sink<T>.Clear(queue);
        }

        protected override void SyncUpdate()
        {
            base.SyncUpdate();
            Sink();
        }
        protected virtual void Sink() => OnSink?.Invoke(this);

        public bool Send(T obj) => Sink<T>.Send(queue, obj);
        public int Send(T[] array, int index, int length) => Sink<T>.Send(queue, array, index, length);
        public int Send(T[] array) => Sink<T>.Send(queue, array);

        protected bool Receive(ref T obj) => Sink<T>.Receive(queue, ref obj);
        protected int Receive(T[] array, int index, int length) => Sink<T>.Receive(queue, array, index, length);
        protected int Receive(T[] array) => Sink<T>.Receive(queue, array);

        bool IUpdatableSink<T>.Receive(ref T obj) => Receive(ref obj);
        int IUpdatableSink<T>.Receive(T[] array) => Receive(array);
        int IUpdatableSink<T>.Receive(T[] array, int index, int length) => Receive(array, index, length);

        public SyncSink() => queue = new DynamicArray<T>(1024 * 1024);
        public SyncSink(IQueue<T> queue) => this.queue = queue;
        public SyncSink(IUpdater updater) : base(updater) => queue = new DynamicArray<T>(1024 * 1024);
        public SyncSink(IUpdater updater, IQueue<T> queue) : base(updater) => this.queue = queue;
    }

    public interface ISyncSink<T> : ISyncObject, IUpdatableSink<T> { }
}
