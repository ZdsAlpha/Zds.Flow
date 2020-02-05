using Flow.Collections;
using Flow.Interfaces;
using Flow.Updatables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Flow.Actors
{
    static class Sink<T>
    {
        public static bool Send(IQueue<T> queue, T obj)
        {
            if (Monitor.TryEnter(queue))
            {
                var output = false;
                try
                {
                    output = queue.Enqueue(obj);
                }
                catch (Exception ex)
                {
                    Monitor.Exit(queue);
                    throw ex;
                }
                Monitor.Exit(queue);
                return output;
            }
            return false;
        }
        public static bool Receive(IQueue<T> queue, ref T obj)
        {
            if (Monitor.TryEnter(queue))
            {
                var output = false;
                try
                {
                    output = queue.Dequeue(ref obj);
                }
                catch (Exception ex)
                {
                    Monitor.Exit(queue);
                    throw ex;
                }
                Monitor.Exit(queue);
                return output;
            }
            return false;
        }
        public static int Send(IQueue<T> queue, T[] array, int index, int length)
        {
            if (index < 0 || index >= array.Length) return 0;
            if (index + length > array.Length) length = array.Length - index; 
            if (length <= 0) return 0;
            if (Monitor.TryEnter(queue))
            {
                int output = 0;
                try
                {
                    if (queue is IBoundedArray<T>)
                        output = ((IBoundedArray<T>)queue).AddLast(array, index, length);
                    else
                        for (int i = index; i < length; i++)
                            if (queue.Enqueue(array[i]))
                                output++;
                            else
                                break;
                }
                catch (Exception ex)
                {
                    Monitor.Exit(queue);
                    throw ex;
                }
                Monitor.Exit(queue);
                return output;
            }
            return 0;
        }
        public static int Receive(IQueue<T> queue, T[] array, int index, int length)
        {
            if (index < 0 || index >= array.Length) return 0;
            if (index + length > array.Length) length = array.Length - index;
            if (length <= 0) return 0;
            if (Monitor.TryEnter(queue))
            {
                int output = 0;
                try
                {
                    if (queue is IBoundedArray<T>)
                        output = ((IBoundedArray<T>)queue).RemoveFirst(array, index, length);
                    else
                        for (int i = index; i < length; i++)
                            if (queue.Dequeue(ref array[i]))
                                output++;
                            else
                                break;
                }
                catch (Exception ex)
                {
                    Monitor.Exit(queue);
                    throw ex;
                }
                Monitor.Exit(queue);
                return output;
            }
            return 0;
        }
        public static int Send(IQueue<T> queue, T[] array) => Send(queue, array, 0, array.Length);
        public static int Receive(IQueue<T> queue, T[] array) => Receive(queue, array, 0, array.Length);
        public static void Clear(IQueue<T> queue)
        {
            lock (queue)
            {
                T obj = default;
                if (obj is IBoundedArray<T>)
                    ((IBoundedArray<T>)obj).Clear();
                else
                    while (queue.Dequeue(ref obj)) ;
            }
        }
    }

    public interface IActor<Message> : ISink<Message> { }

    public interface IUpdatableSink<T> : IUpdatable, ISink<T>, IActor<T>
    {
        int Send(T[] array);
        int Send(T[] array, int index, int length);
        bool Receive(ref T obj);
        int Receive(T[] array);
        int Receive(T[] array, int index, int length);
    }

    public delegate void ActorDelegate<T>(IUpdatableSink<T> sender);

    public delegate void SinkDelegate<T>(T obj);
}
