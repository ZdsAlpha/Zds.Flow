using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Interfaces
{
    public interface ITimer : IStartStopable
    {
        TimeSpan Delay { get; set; }
    }
}
