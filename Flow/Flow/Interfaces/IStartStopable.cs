using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Interfaces
{
    public interface IStartStopable
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
        void Reset();
    }
}
