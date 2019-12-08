using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Collections
{
    public class DynamicArray<T> : IDynamicArray<T>, IQueue<T>, IStack<T>, IStream<T>
    {
        const double GROWTH_FACTOR = 2;
        private readonly BoundedArray<T> array = new BoundedArray<T>(1);
        public int MaxSize { get; set; } = int.MaxValue;
        public int Length => array.Length;
        public int Size => array.Size;
        public int FreeSpace => MaxSize - Length;
        public T this[int index]
        {
            get => array[index];
            set => array[index] = value;
        }
        public int SetSize(int size)
        {
            if (size > MaxSize) size = MaxSize;
            if (size <= 1) size = 1;
            return array.SetSize(size);
        }
        public int EnsureLength(int length)
        {
            if (length <= array.Size) return array.Size;
            long size = (long)Math.Ceiling(array.Size * GROWTH_FACTOR);
            if (size > MaxSize) size = MaxSize;
            return SetSize((int)size);
        }
        public int CopyTo(T[] destination, int sourceIndex, int destinationIndex, int length)
        {
            return array.CopyTo(destination, sourceIndex, destinationIndex, length);
        }
        public int CopyFrom(T[] source, int sourceIndex, int destinationIndex, int length)
        {
            return array.CopyFrom(source, sourceIndex, destinationIndex, length);
        }
        public int ExtendFirst(int count)
        {
            EnsureLength(Length + count);
            return array.ExtendFirst(count);
        }
        public int ExtendLast(int count)
        {
            EnsureLength(Length + count);
            return array.ExtendLast(count);
        }
        public int ShrinkFirst(int count) => array.ShrinkFirst(count);
        public int ShrinkLast(int count) => array.ShrinkLast(count);
        public bool AddFirst(T element)
        {
            EnsureLength(Length + 1);
            return array.AddFirst(element);
        }
        public bool AddLast(T element)
        {
            EnsureLength(Length + 1);
            return array.AddLast(element);
        }
        public int AddFirst(T[] source, int index, int length)
        {
            if (index < 0 || length <= 0) return 0;
            if (index >= source.Length) return 0;
            if (index + length > source.Length) length = source.Length - index;
            EnsureLength(Length + length);
            return array.AddFirst(source, index, length);
        }
        public int AddLast(T[] source, int index, int length)
        {
            if (index < 0 || length <= 0) return 0;
            if (index >= source.Length) return 0;
            if (index + length > source.Length) length = source.Length - index;
            EnsureLength(Length + length);
            return array.AddLast(source, index, length);
        }
        public bool RemoveFirst() => array.RemoveFirst();
        public bool RemoveLast() => array.RemoveLast();
        public int RemoveFirst(int count) => array.RemoveFirst(count);
        public int RemoveLast(int count) => array.RemoveLast(count);
        public bool RemoveFirst(ref T element) => array.RemoveFirst(ref element);
        public bool RemoveLast(ref T element) => array.RemoveLast(ref element);
        public int RemoveFirst(T[] destination, int index, int length)
        {
            return array.RemoveFirst(destination, index, length);
        }
        public int RemoveLast(T[] destination, int index, int length)
        {
            return array.RemoveLast(destination, index, length);
        }

        public bool Enqueue(T element) => array.Enqueue(element);
        public bool Dequeue(ref T element) => array.Dequeue(ref element);

        public bool Push(T element) => array.Push(element);
        public bool Pop(ref T element) => array.Pop(ref element);

        public int Write(T[] array, int index, int length) => this.array.Write(array, index, length);
        public int Read(T[] array, int index, int length) => this.array.Read(array, index, length);

        public bool Get(int index, ref T element) => array.Get(index, ref element);
        public bool Set(int index, T element) => array.Set(index, element);
        public void Clear() => array.Clear();
        public T[] ToArray() => array.ToArray();
        public DynamicArray() { }
        public DynamicArray(int size) => array.SetSize(size);
    }

    public interface IDynamicArray<T> : IBoundedArray<T> { };
}
