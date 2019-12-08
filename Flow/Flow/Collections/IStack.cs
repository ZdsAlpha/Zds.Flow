using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Collections
{
    public interface IStack<T>
    {
        bool Push(T element);
        bool Pop(ref T element);
    }
}
