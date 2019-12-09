using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Flow.Collections
{
    public class SafeList<T> : IList<T>
    {
        private readonly List<T> list;
        private T[] cache;
        public T[] Elements => cache;
        public T this[int index]
        {
            get => cache[index];
            set
            {
                lock (list)
                {
                    list[index] = value;
                    UpdateCache();
                }
            }
        }
        public object Lock => list;
        public int Count => cache.Length;
        public bool IsReadOnly => false;
        public void Add(T item)
        {
            lock (list)
            {
                list.Add(item);
                UpdateCache();
            }
        }
        public void AddRange(T[] items)
        {
            lock (list)
            {
                list.AddRange(items);
                UpdateCache();
            }
        }
        public void Insert(int index, T item)
        {
            lock (list)
            {
                list.Insert(index, item);
                UpdateCache();
            }
        }
        public bool Remove(T item)
        {
            lock (list)
            {
                bool output = list.Remove(item);
                UpdateCache();
                return output;
            }
        }
        public void RemoveAll(Predicate<T> match)
        {
            lock (list)
            {
                list.RemoveAll(match);
                UpdateCache();
            }
        }
        public void RemoveAt(int index)
        {
            lock (list)
            {
                list.RemoveAt(index);
                UpdateCache();
            }
        }
        public int IndexOf(T item)
        {
            return Array.IndexOf(cache, item);
        }
        public bool Contains(T item)
        {
            return IndexOf(item) > -1;
        }
        public void Clear()
        {
            lock (list)
            {
                list.Clear();
                UpdateCache();
            }
        }
        public void CopyTo(T[] target, int count)
        {
            cache.CopyTo(target, count);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<T> GetEnumerator()
        {
            var collection = cache;
            for (int i = 0; i < collection.Length; i++)
                yield return collection[i];
        }
        public void UpdateCache()
        {
            lock (list)
            {
                cache = list.ToArray();
            }
        }

        public SafeList()
        {
            list = new List<T>();
            cache = new T[] { };
        }
    }
}
