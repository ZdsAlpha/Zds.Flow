using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Interfaces
{
    public interface ISink<T>
    {
        bool Send(T obj);
    }
}
