using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Collections
{
    public interface IStream<T>
    {
        int Write(T[] array, int index, int lenght);
        int Read(T[] array, int index, int length);
    }
}
