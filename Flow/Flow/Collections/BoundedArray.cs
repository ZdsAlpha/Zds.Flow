using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Collections
{
    public class BoundedArray<T> : IBoundedArray<T>, IQueue<T>, IStack<T>, IStream<T>
    {
        private T[] buffer;
        private int position;
        private int length;
        public int Length => length;
        public int Size => buffer.Length;
        public int FreeSpace => buffer.Length - length;
        public T this[int index]
        {
            get
            {
                T element = default;
                if (!Get(index, ref element)) throw new IndexOutOfRangeException();
                return element;
            }
            set
            {
                if (!Set(index, value)) throw new IndexOutOfRangeException();
            }
        }
        public int SetSize(int size)
        {
            if (size < length) size = length;
            if (size == buffer.Length) return size;
            T[] _buffer = new T[size];
            CopyTo(_buffer, 0, 0, length);
            buffer = _buffer;
            position = 0;
            return size;
        }
        public int CopyTo(T[] destination, int sourceIndex, int destinationIndex, int length)
        {
            if (sourceIndex < 0 || destinationIndex < 0 || length <= 0) return 0;
            if (sourceIndex >= this.length) return 0;
            if (destinationIndex >= destination.Length) return 0;
            if (sourceIndex + length > this.length) length = this.length - sourceIndex;
            if (destinationIndex + length > destination.Length) length = destination.Length - destinationIndex;
            if (position + sourceIndex < buffer.Length && position + sourceIndex + length > buffer.Length){
                Array.Copy(buffer, position + sourceIndex, destination, destinationIndex, buffer.Length - position - sourceIndex);
                Array.Copy(buffer, 0, destination, destinationIndex + buffer.Length - position - sourceIndex, length + position + sourceIndex - buffer.Length);
            }
            else
                Array.Copy(buffer, Modulo(position + sourceIndex, buffer.Length), destination, destinationIndex, length);
            return length;
        }
        public int CopyFrom(T[] source, int sourceIndex, int destinationIndex, int length)
        {
            if (sourceIndex < 0 || destinationIndex < 0 || length <= 0) return 0;
            if (sourceIndex >= source.Length) return 0;
            if (destinationIndex >= this.length) return 0;
            if (sourceIndex + length > source.Length) length = source.Length - sourceIndex;
            if (destinationIndex + length > this.length) length = this.length - destinationIndex;
            if (position + destinationIndex < buffer.Length && position + destinationIndex + length > buffer.Length)
            {
                Array.Copy(source, sourceIndex, buffer, position + destinationIndex, buffer.Length - position - destinationIndex);
                Array.Copy(source, sourceIndex + buffer.Length - position - destinationIndex, buffer, 0, length + position + destinationIndex - buffer.Length);
            }
            else
                Array.Copy(source, sourceIndex, buffer, Modulo(position + destinationIndex, buffer.Length), length);
            return length;
        }
        public int ExtendFirst(int count)
        {
            if (count <= 0) return 0;
            if (count > FreeSpace) count = FreeSpace;
            position = Modulo(position - count, buffer.Length);
            length += count;
            return count;
        }
        public int ExtendLast(int count)
        {
            if (count <= 0) return 0;
            if (count > FreeSpace) count = FreeSpace;
            length += count;
            return count;
        }
        public int ShrinkFirst(int count)
        {
            if (count <= 0) return 0;
            if (count > length) count = length;
            if (position + count > buffer.Length)
            {
                Array.Clear(buffer, position, buffer.Length - position);
                Array.Clear(buffer, 0, count + position - buffer.Length);
            }
            else
                Array.Clear(buffer, position, count);
            position = Modulo(position + count, buffer.Length);
            length -= count;
            return count;
        }
        public int ShrinkLast(int count)
        {
            if (count <= 0) return 0;
            if (count > length) count = length;
            if (position + length > buffer.Length && position + length - count < buffer.Length)
            {
                Array.Clear(buffer, position + length - count, buffer.Length - position - length + count);
                Array.Clear(buffer, 0, position + length - buffer.Length);
            }
            else
                Array.Clear(buffer, Modulo(position + length - count, buffer.Length), count);
            length -= count;
            return count;
        }
        public bool AddFirst(T element)
        {
            if (ExtendFirst(1) == 0) return false;
            buffer[position] = element;
            return true;
        }
        public bool AddLast(T element)
        {
            if (ExtendLast(1) == 0) return false;
            buffer[Modulo(position + length - 1, buffer.Length)] = element;
            return true;
        }
        public int AddFirst(T[] source, int index, int length)
        {
            if (index < 0 || length <= 0) return 0;
            if (index >= source.Length) return 0;
            if (index + length > source.Length) length = source.Length - index;
            length = ExtendFirst(length);
            return CopyFrom(source, index, 0, length);
        }
        public int AddLast(T[] source, int index, int length)
        {
            if (index < 0 || length <= 0) return 0;
            if (index >= source.Length) return 0;
            if (index + length > source.Length) length = source.Length - index;
            length = ExtendLast(length);
            return CopyFrom(source, index, this.length - length, length);
        }
        public bool RemoveFirst() => ShrinkFirst(1) == 1;
        public bool RemoveLast() => ShrinkLast(1) == 1;
        public int RemoveFirst(int count) => ShrinkFirst(count);
        public int RemoveLast(int count) => ShrinkLast(count);
        public bool RemoveFirst(ref T element)
        {
            if (length == 0) return false;
            element = buffer[position];
            ShrinkFirst(1);
            return true;
        }
        public bool RemoveLast(ref T element)
        {
            if (length == 0) return false;
            element = buffer[Modulo(position + length - 1, buffer.Length)];
            ShrinkLast(1);
            return true;
        }
        public int RemoveFirst(T[] destination, int index, int length)
        {
            if (index < 0 || length <= 0) return 0;
            if (index >= destination.Length) return 0;
            if (length > this.length) length = this.length;
            if (index + length > destination.Length) length = destination.Length - index;
            CopyTo(destination, 0, index, length);
            return ShrinkFirst(length);
        }
        public int RemoveLast(T[] destination, int index, int length)
        {
            if (index < 0 || length <= 0) return 0;
            if (index >= destination.Length) return 0;
            if (length > this.length) length = this.length;
            if (index + length > destination.Length) length = destination.Length - index;
            CopyTo(destination, this.length - length, index, length);
            return ShrinkLast(length);
        }

        public bool Enqueue(T element) => AddLast(element);
        public bool Dequeue(ref T element) => RemoveFirst(ref element);

        public bool Push(T element) => AddLast(element);
        public bool Pop(ref T element) => RemoveLast(ref element);

        public int Write(T[] array, int index, int length) => AddLast(array, index, length);
        public int Read(T[] array, int index, int length) => RemoveFirst(array, index, length);

        public bool Get(int index, ref T element)
        {
            if (index < 0 || index >= length) return false;
            element = buffer[Modulo(position + index, buffer.Length)];
            return true;
        }
        public bool Set(int index, T element)
        {
            if (index < 0 || index >= length) return false;
            element = buffer[Modulo(position + index, buffer.Length)] = element;
            return true;
        }
        public void Clear()
        {
            ShrinkLast(length);
        }
        public T[] ToArray()
        {
            T[] array = new T[length];
            CopyTo(array, 0, 0, length);
            return array;
        }
        public BoundedArray(int size)
        {
            buffer = new T[size];
            position = 0;
            length = 0;
        }
        private static int Modulo(int dividend,int divisor)
        {
            return (dividend % divisor + divisor) % divisor;
        }
    }

    public interface IBoundedArray<T>
    {
        T this[int index] { get; set; }
        int ExtendFirst(int count);
        int ExtendLast(int count);
        int ShrinkFirst(int count);
        int ShrinkLast(int count);
        bool AddFirst(T element);
        bool AddLast(T element);
        int AddFirst(T[] source, int index, int length);
        int AddLast(T[] source, int index, int length);
        bool RemoveFirst(ref T element);
        bool RemoveLast(ref T element);
        int RemoveFirst(T[] destination, int index, int length);
        int RemoveLast(T[] destination, int index, int length);
        void Clear();
    }
}
