using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Collections
{
    public interface IQueue<T>
    {
        bool Enqueue(T element);
        bool Dequeue(ref T element);
    }
}
